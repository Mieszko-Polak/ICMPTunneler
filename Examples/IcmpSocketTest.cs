using System.Net;
using System.Net.Sockets;

public class IcmpSocketTest
{
    public async Task SendIcmpTest(IPAddress server)
    {
        using Socket sender = new(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        //string message = "Testing";
        //sender.Connect(IPAddress.Parse("127.0.0.1").Serialize);
    }
}