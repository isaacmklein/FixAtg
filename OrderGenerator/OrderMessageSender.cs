using QuickFix;

namespace OrderGenerator
{
    public static class OrderMessageSender
    {
        public static void SendOrder(QuickFix.FIX44.NewOrderSingle order)
        {
            try
            {
                // Serializa o objeto FIX para uma string
                string orderString = order.ToString();

                SendMessage(orderString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar ordem: {ex.Message}");
            }
        }

        private static void SendMessage(string message)
        {
            try
            {
                SessionID sessionID = new SessionID("FIX.4.4", "OrderGenerator", "OrderAccumulator");
                QuickFix.Message fixMessage = new QuickFix.Message(message);

                Session.SendToTarget(fixMessage, sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem para OrderAccumulator: {ex.Message}");
            }
        }
    }
}
