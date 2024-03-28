using System;
using System.Threading;
using QuickFix;
using QuickFix.Fields;
using QuickFix.Transport;

namespace OrderGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SessionSettings settings = new SessionSettings("ordergenerator.cfg");
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                IMessageFactory messageFactory = new DefaultMessageFactory();

                OrderGeneratorApp application = new OrderGeneratorApp();
                SocketInitiator initiator = new SocketInitiator(application, storeFactory, settings, logFactory, messageFactory);
                initiator.Start();

                Console.WriteLine("Order Generator iniciado. Pressione qualquer tecla para sair.");

                // Loop para gerar ordens a cada segundo
                while (true)
                {
                    GenerateAndSendOrder(application);
                    Thread.Sleep(1000); // Aguarda 1 segundo
                }

                // Mantém o programa em execução até que uma tecla seja pressionada
                Console.ReadKey();

                initiator.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }

        static void GenerateAndSendOrder(OrderGeneratorApp application)
        {
            // Gera uma nova ordem com as propriedades especificadas
            string symbol = GetRandomSymbol();
            char side = GetRandomSide();
            decimal quantity = GetRandomQuantity();
            decimal price = GetRandomPrice();

            // Imprime as informações da ordem de forma estruturada
            Console.WriteLine("Nova ordem gerada:");
            Console.WriteLine($"  Symbol: {symbol}");
            string sideString = side == '1' ? "BUY" : "SELL";
            Console.WriteLine($"Side: {sideString}");
            //Console.WriteLine($"  Side: {side}"); Comentado pois optei por exibir a descrição acima
            Console.WriteLine($"  OrderQty: {quantity}");
            Console.WriteLine($"  Price: {price}");

            // Cria a mensagem FIX NewOrderSingle
            QuickFix.FIX44.NewOrderSingle order = new QuickFix.FIX44.NewOrderSingle(
                new ClOrdID(Guid.NewGuid().ToString()),
                new Symbol(symbol),
                new Side(side),
                new TransactTime(DateTime.UtcNow),
                new OrdType(OrdType.LIMIT)
            );

            order.OrderQty = new OrderQty(quantity);
            order.Price = new Price(price);

            // Envia a ordem
            Console.WriteLine("Enviando ordem:");
            Console.WriteLine(order.ToString());
            Console.WriteLine();
            // Chame o método para enviar a ordem
            OrderMessageSender.SendOrder(order);
        }

        static string GetRandomSymbol()
        {
            string[] symbols = { "PETR4", "VALE3", "VIIA4" };
            Random random = new Random();
            return symbols[random.Next(symbols.Length)];
        }

        static char GetRandomSide()
        {
            Random random = new Random();
            return random.Next(2) == 0 ? Side.BUY : Side.SELL;
        }

        static decimal GetRandomQuantity()
        {
            Random random = new Random();
            return random.Next(1, 100000); // Mínimo de 1 para garantir que seja menor que 100.000
        }

        static decimal GetRandomPrice()
        {
            Random random = new Random();
            return Math.Round((decimal)(random.Next(1, 100000) * 0.01), 2); // Mínimo de 0.01
        }
    }
}
