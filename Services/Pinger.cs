using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ICMPTunneler.Services;

public class Pinger
{
    public static async Task SendPing(string destination, string message, int disruptionChance = 0)
    {
    var random = new Random();
    if (random.Next(disruptionChance) != 1)
    {
        Ping sender = new Ping();
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        int timeout = 128;
        PingReply reply = sender.Send(IPAddress.Parse(destination), timeout,messageBytes);
        //Console.WriteLine(reply);
    } else
    {
        Console.WriteLine("Packet with message " + message + " got disrupted");
    }
    }
}