using QuickFix;
using QuickFix.Fields;
using OrderAccumulator.Services;

namespace OrderAccumulator
{
    class OrderAccumulatorApp : QuickFix.MessageCracker, IApplication
    {
        private readonly OrderAccumulatorService _orderAccumulatorService = new OrderAccumulatorService();
        private SessionID _sessionID = new SessionID("FIX.4.4", "OrderAccumulator","OrderGenerator");

        public void FromAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("FromAdmin: " + message.ToString());
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("FromApp: " + message.ToString());
            try
            {
                // Processar a mensagem recebida do OrderGenerator
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao processar mensagem: " + ex.Message);
            }
        }

        public void OnCreate(SessionID sessionID)
        {
            _sessionID = sessionID;
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine("OnLogon: " + sessionID.ToString());
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine("OnLogout: " + sessionID.ToString());
        }

        public void ToAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("ToAdmin: " + message.ToString());
        }

        public void ToApp(Message message, SessionID sessionId)
        {
            Console.WriteLine("ToApp: " + message.ToString());
        }

        public void OnMessage(QuickFix.FIX44.NewOrderSingle newOrder, SessionID sessionID)
        {           
        }

        public void SendExecutionReport(Order order, char ordStatus)
        {
            try
            {
                QuickFix.FIX44.ExecutionReport executionReport = new QuickFix.FIX44.ExecutionReport();

                executionReport.Set(new ClOrdID(order.ClOrdID)); // Utiliza o ClOrdID da ordem
                executionReport.Set(new OrderID(Guid.NewGuid().ToString())); // Gera um novo OrderID
                executionReport.Set(new ExecID(Guid.NewGuid().ToString()));
                executionReport.Set(new ExecType(ExecType.NEW));

                executionReport.Set(new ExecType(ordStatus));
                executionReport.Set(new OrdStatus(ordStatus));
                executionReport.Set(new Symbol(order.Symbol));
                executionReport.Set(new Side(order.IsBuy ? Side.BUY : Side.SELL));
                executionReport.Set(new LeavesQty(order.Quantity));
                executionReport.Set(new CumQty(0));
                executionReport.Set(new AvgPx(0));

                executionReport.Set(new LastPx(0));
                executionReport.Set(new OrderQty(order.Quantity));
                executionReport.Set(new Price(order.Price));
                executionReport.Set(new TransactTime(DateTime.UtcNow));

                Session.SendToTarget(executionReport, _sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar relatório de execução: {ex.Message}");
            }
        }


        public void SendOrderReject(Order order, string reason)
        {
            QuickFix.FIX44.OrderCancelReject orderReject = new QuickFix.FIX44.OrderCancelReject(
                new OrderID(order.OrderID),
                new ClOrdID(order.ClOrdID),
                new OrigClOrdID(order.ClOrdID),
                new OrdStatus(OrdStatus.REJECTED),
                new CxlRejResponseTo(CxlRejResponseTo.ORDER_CANCEL_REQUEST)
            );

            Session.SendToTarget(orderReject, _sessionID);
        }
    }
}
