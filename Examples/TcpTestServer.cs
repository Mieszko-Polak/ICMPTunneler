using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ICMPTunneler.Examples;
public class TcpTestServer
{
    public async void StartServer()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.IPv6Any, 13000);
        TcpListener listener = new(ipEndPoint);
        listener.Server.DualMode = true;

        try
        {
            listener.Start();

            using TcpClient handler = await listener.AcceptTcpClientAsync();
            await using NetworkStream stream = handler.GetStream();

            var message = "I can hear you";
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageBytes);

            Console.WriteLine($"Server sent message: {message}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            listener.Stop();
        }
    }

}