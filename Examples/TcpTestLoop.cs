using System.Net.Sockets;
using System.Net;
using ICMPTunneler.Examples;

namespace ICMPTunneler.Examples;
public class TcpTestLoop
{
    public static async Task SendToLocalhost()
    {
        var tcpServer = new TcpTestServer();
        tcpServer.StartServer();
        TcpTestClient.SendTestPacket("127.0.0.1", "Can you hear me through local stuff?");
    }

    public static async Task SendToRouter4()
    {
        var tcpServer = new TcpTestServer();
        tcpServer.StartServer();
        var hostName = Dns.GetHostName();
        IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
        IPAddress localIpAddress = localhost.AddressList[1];
        TcpTestClient.SendTestPacket(localIpAddress.ToString(), "Can you hear me through the router, in IPv4?");
    }

    public static async Task SendToRouter6()
    {
        var tcpServer = new TcpTestServer();
        tcpServer.StartServer();

        var hostName = Dns.GetHostName();
        IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
        IPAddress localIpAddress = localhost.AddressList[0];
        TcpTestClient.SendTestPacket(localIpAddress.ToString(), "Can you hear me through the router, in IPv6?");
    }
}