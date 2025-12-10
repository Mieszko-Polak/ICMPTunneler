using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ICMPTunneler.Examples;
public class IcmpSocketTest
{
    public static async Task SendIcmpTest(IPAddress server)
    {
        using Socket sender = new(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        sender.DontFragment = false;
        string message = "T";
        byte[] messageBytes = Encoding.ASCII.GetBytes (message);
        sender.Connect(server, 0);
        sender.Send(messageBytes);
        Console.WriteLine($"Outside sent {message} as ICMP packet");
    }
}