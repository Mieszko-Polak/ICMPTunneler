using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace ICMPTunneler.Services;

public class Pinger
{
    public static async Task SendPing(string destination, string message)
    {
        Ping sender = new Ping();
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        int timeout = 128;
        PingReply reply = sender.Send(IPAddress.Parse(destination), timeout,messageBytes);
    }
}