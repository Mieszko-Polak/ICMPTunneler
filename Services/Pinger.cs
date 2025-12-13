using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ICMPTunneler.Services;

public class Pinger
{
    public static async Task SendPing(string destination, string message, int OneOverDisruptionChance = 0)
    {
    var random = new Random();
    //A 1/disruption chance to not send the ping (unless its 0, then the ping is sent
    if (random.Next(OneOverDisruptionChance) != 1)
    {
        Ping sender = new Ping();
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        int timeout = 128;
        //Send the ping. Reply isn't used, but might as well collect it.
        PingReply reply = sender.Send(IPAddress.Parse(destination), timeout,messageBytes);
    } else
    {
        Console.WriteLine("Packet with message " + message + " got disrupted");
    }
    }
}