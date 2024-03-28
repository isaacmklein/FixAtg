using OrderAccumulator.Services;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class OrderMessageReceiver
{
    private const int Port = 9876; // Porta usada pelo OrderGenerator
    private readonly OrderAccumulatorService _orderAccumulatorService = new OrderAccumulatorService();

    public void StartReceiving()
    {
        Console.WriteLine("Iniciando o processo de recebimento de mensagens...");

        // Configurar um listener TCP para aguardar conexões do OrderGenerator
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();

        try
        {
            // Aguardar e aceitar uma conexão do OrderGenerator
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Conexão estabelecida com OrderGenerator.");

            // Obter a stream de dados do cliente para receber mensagens
            NetworkStream stream = client.GetStream();

            // Loop infinito para receber mensagens do OrderGenerator
            while (true)
            {
                // Use o MessageFactory para decodificar a próxima mensagem
                byte[] buffer = ReadMessageBytes(stream);
                string messageString = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                QuickFix.Message fixMessage = new QuickFix.Message(messageString);

                // Processar a mensagem recebida
                ReceiveMessage(fixMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao receber mensagens do OrderGenerator: {ex.Message}");
        }
        finally
        {
            // Parar o listener quando terminar
            listener.Stop();
        }
    }

    private static byte[] ReadMessageBytes(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        Array.Resize(ref buffer, bytesRead);

        return buffer;
    }


    public void ReceiveMessage(QuickFix.Message message)
    {
        
        var order = ParseToOrder(message);
        if (order != null)
        {
            _orderAccumulatorService.ProcessOrder(new Order(order));
        }
    }

    public static QuickFix.FIX44.NewOrderSingle ParseToOrder(QuickFix.Message message)
    {
        try
        {
            string symbol = message.GetField(Tags.Symbol);
            char side = message.GetField(Tags.Side)[0];
            int quantity = message.GetInt(Tags.OrderQty);
            decimal price = message.GetDecimal(Tags.Price);

            QuickFix.Message order = new QuickFix.FIX44.NewOrderSingle(
                new ClOrdID(Guid.NewGuid().ToString()),
                new Symbol(symbol),
                new Side(side),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.LIMIT)
            );

            order.SetField(new OrderQty(quantity));
            order.SetField(new Price(price));

            return (NewOrderSingle)order;
        }
        catch (QuickFix.UnsupportedMessageType)
        {
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
