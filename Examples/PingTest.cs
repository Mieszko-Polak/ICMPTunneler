using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ICMPTunneler.Examples;
public class PingTest
{
    public static async Task SendPing()
    {
        Ping sender = new Ping();
        string message = "Testing Things very very well";
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        int timeout = 128;
        PingReply reply = sender.Send(IPAddress.Parse("192.168.1.76"), timeout,messageBytes);
        if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine ("Address: {0}", reply.Address.ToString ());
                Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine ("Buffer size: {0}", reply.Buffer.Length);
                Console.WriteLine ($"Buffer Contents: {Encoding.ASCII.GetString(reply.Buffer)}");
            }
        else
        {
            Console.WriteLine ("Reply States: {0}", reply.Status);
        }
    }
}