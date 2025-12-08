using System.Net.Sockets;
using System.Net;
using ICMPTunneler.Examples;

namespace ICMPTunneler.Examples;
public class SocketTestLoop
{
    public static async Task SendToLocalhost()
    {
        var serverTask = Task.Run(() => SocketServerTest.StartServer());
        await Task.Delay(100);
        await SocketClientTest.SendPacket(IPAddress.Parse("127.0.0.1"), "Can you hear me through local stuff?");
    }

    public static async Task SendToRouter4()
    {
        var serverTask = Task.Run(() => SocketServerTest.StartServer());
        IPHostEntry localhost = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress localIpAddress = localhost.AddressList[1];
        await SocketClientTest.SendPacket(localIpAddress, "Can you hear me through the router, in IPv4?");
    }

    public static async Task SendToRouter6()
    {
        var serverTask = Task.Run(() => SocketServerTest.StartServer());
        IPHostEntry localhost = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress localIpAddress = localhost.AddressList[0];
        await SocketClientTest.SendPacket(localIpAddress, "Can you hear me through the router, in IPv6?");
    }
}