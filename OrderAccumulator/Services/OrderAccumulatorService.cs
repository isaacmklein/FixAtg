using System;
using OrderAccumulator.Domain.Entities;
using QuickFix.Fields;
using QuickFix.FIX44;
using QuickFix;

namespace OrderAccumulator.Services
{
    public class OrderAccumulatorService : IOrderAccumulatorService
    {
        public readonly Dictionary<string, Exposure> _exposures;
        private const decimal AbsoluteExposureLimit = 1000000;

        public OrderAccumulatorService()
        {
            _exposures = new Dictionary<string, Exposure>
            {
                { "PETR4", new Exposure { Symbol = "PETR4", Value = 0m } },
                { "VALE3", new Exposure { Symbol = "VALE3", Value = 0m } },
                { "VIIA4", new Exposure { Symbol = "VIIA4", Value = 0m } }
            };
        }

        public Exposure GetExposure(string symbol)
        {
            return _exposures[symbol];
        }

        public bool ProcessOrder(Order order)
        {
            decimal orderValue = order.Price * order.Quantity;
            Exposure exposure = _exposures[order.Symbol];

            Console.WriteLine("Processando ordem...");
            Console.WriteLine($"Order Symbol: {order.Symbol}, Quantity: {order.Quantity}, Price: {order.Price}");

            // Calcula a exposição financeira atual para o símbolo
            decimal currentExposure = exposure.Value + (order.IsBuy ? orderValue : -orderValue);

            // Verifica se a ordem ultrapassa o limite de exposição em valor absoluto
            bool exceedsAbsoluteLimit = Math.Abs(currentExposure) > AbsoluteExposureLimit;

            // Verifica se a ordem deve ser aceita ou rejeitada
            bool isOrderAccepted = !exceedsAbsoluteLimit;
            OrderAccumulatorApp app = new OrderAccumulatorApp();
            // Atualiza a exposição financeira apenas se a ordem for aceita
            if (isOrderAccepted)
            {
                exposure.Value = currentExposure;
                app.SendExecutionReport(order, QuickFix.Fields.OrdStatus.NEW);
            }
            else
            {
                app.SendOrderReject(order, "Ordem rejeitada por exceder o limite de exposição financeira.");
            }

            // Exibe o resultado da validação e atualização da exposição financeira
            if (isOrderAccepted)
            {
                Console.WriteLine($"Ordem {(order.IsBuy ? "de compra" : "de venda")} aceita...");
            }
            else
            {
                Console.WriteLine($"Ordem {(order.IsBuy ? "de compra" : "de venda")} rejeitada por exceder o limite de exposição financeira...");
            }

            Console.WriteLine($"Exposição financeira atual para {order.Symbol}: {currentExposure}");
            Console.WriteLine();

            return isOrderAccepted;
        }
    }
}
