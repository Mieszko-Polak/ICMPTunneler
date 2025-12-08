using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ICMPTunneler.Examples;
public class SocketServerTest
{
    public static async Task StartServer()
    {
        
        IPEndPoint ipEndPoint = new(IPAddress.IPv6Any, 12000);
        using Socket listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only,0);

        listener.Bind(ipEndPoint);
        listener.Listen(100);

        var handler = await listener.AcceptAsync();
        while (true)
        {
            var buffer = new byte[1024];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);

            var eom = "<|EOM|>";
            if (response.IndexOf(eom) > -1)
            {
                Console.WriteLine($"Server received: {response.Replace(eom, "")}");

                var ackMessage = "<|ACK|>";
                var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                await handler.SendAsync(echoBytes, SocketFlags.None);
                Console.WriteLine("Server sent acknowledgement");
                break;
            }
        }
    }
}