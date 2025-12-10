using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ICMPTunneler.Examples;
public class IcmpListenerTest
{
    public static async Task listen()
    {
        Socket listener = new(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        listener.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.76"), 0));
        listener.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });

        byte[] buffer = new byte[4096];
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        int bytesRead = listener.ReceiveFrom(buffer, ref remoteEndPoint);
        Console.WriteLine("ICMPListener received " + bytesRead + " from " + remoteEndPoint);
        Console.ReadLine();
    }
    
}