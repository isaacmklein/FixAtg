using QuickFix;
namespace OrderAccumulator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var settings = new SessionSettings("OrderAccumulator.cfg");
                var orderAccumulatorApp = new OrderAccumulatorApp();
                var storeFactory = new FileStoreFactory(settings);
                var logFactory = new FileLogFactory(settings);
                var acceptor = new ThreadedSocketAcceptor(orderAccumulatorApp, storeFactory, settings, logFactory);

                acceptor.Start();

                Console.WriteLine("Order Accumulator iniciado. Pressione qualquer tecla para sair...");

                OrderMessageReceiver receiver = new OrderMessageReceiver();
                receiver.StartReceiving();

                Console.ReadKey();

                acceptor.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }
    }
}
