using QuickFix;
using QuickFix.Fields;

namespace OrderGenerator
{
    class OrderGeneratorApp : MessageCracker, IApplication
    {
        private SessionID _sessionID;
        private Timer _orderTimer;
        private Random _random;

        public OrderGeneratorApp()
        {
            _random = new Random();
        }

        public void FromAdmin(Message message, SessionID sessionID)
        {
        }

        public void FromApp(Message message, SessionID sessionID)
        {
        }

        public void OnCreate(SessionID sessionID)
        {
            _sessionID = sessionID;
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine("OnLogon: " + sessionID.ToString());

            // Verifica se o temporizador já foi inicializado antes de criar um novo
            if (_orderTimer == null)
            {
                _orderTimer = new Timer(GenerateOrder, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
            else
            {
                // Se o temporizador já estiver inicializado, reinicie-o
                _orderTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine("OnLogout: " + sessionID.ToString());

            // Verifica se o temporizador foi inicializado antes de tentar liberá-lo
            if (_orderTimer != null)
            {
                _orderTimer.Dispose();
            }
        }

        public void ToAdmin(Message message, SessionID sessionID)
        {
        }

        public void ToApp(Message message, SessionID sessionID)
        {
        }

        private void GenerateOrder(object state)
        {
            if (_orderTimer == null)
            {
                Console.WriteLine("Erro: Timer não inicializado corretamente.");
                return;
            }

            string[] symbols = { "PETR4", "VALE3", "VIIA4" };
            char[] sides = { Side.BUY, Side.SELL };

            string symbol = symbols[_random.Next(symbols.Length)];
            char side = sides[_random.Next(sides.Length)];
            int quantity = _random.Next(1, 100000);
            decimal price = Math.Round((decimal)_random.NextDouble() * 999.99m, 2);

            Message order = new QuickFix.FIX44.NewOrderSingle(
                new ClOrdID(Guid.NewGuid().ToString()),
                new Symbol(symbol),
                new Side(side),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.LIMIT)
            );

            order.SetField(new OrderQty(quantity));
            order.SetField(new Price(price));

            Session.SendToTarget(order, _sessionID);
        }
    }
}
