using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ICMPTunneler.Examples;
class SocketClientTest()
{
    public static async Task SendPacket(IPAddress server, string message)
    {
        IPEndPoint ipEndPoint = new(server, 12000);
        using Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        await client.ConnectAsync(ipEndPoint);
        Byte[] messageBytes = Encoding.UTF8.GetBytes(message + "<|EOM|>");
        while (true)
        {
            await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine($"Client sent Message: {message}");

            var buffer = new byte[1024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            if (response == "<|ACK|>")
            {
                Console.WriteLine($"Client recieved acknowledgment, with response: {response}");
                break;
            }
        }
    }
}

