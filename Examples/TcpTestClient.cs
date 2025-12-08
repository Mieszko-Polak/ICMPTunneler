using System.Net.Sockets;

namespace ICMPTunneler.Examples;
public class TcpTestClient
{
    public static async void SendTestPacket(string server, string message)
    {
        try
        {
            Int32 port = 13000;
            using TcpClient client = new TcpClient(server, port);

            Byte[] data  = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);

            data = new byte[256];

            String responseData = String.Empty;

            Int32 bytes = stream.Read(data, 0 , data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0 , bytes);
            Console.WriteLine("Recieved {0}", responseData);
        }   
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }
}
