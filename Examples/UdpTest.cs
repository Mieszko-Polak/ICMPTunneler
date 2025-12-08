using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ICMPTunneler.Examples;
public class UdpTest
{

    public void SendTestPacket()
    {
        UdpClient udpClientA = new UdpClient(11000);
        try{
            udpClientA.Connect("127.0.0.1", 11000);

            Byte[] sendBytesA = Encoding.ASCII.GetBytes("Is anybody there?");

            udpClientA.Send(sendBytesA, sendBytesA.Length);

            UdpClient udpClientB = new UdpClient();  //No port, so randomly assigned one, and would be no good for listening
            Byte[] sendBytesB = Encoding.ASCII.GetBytes("or am I alone?");
            udpClientB.Send(sendBytesB, sendBytesB.Length, "127.0.0.1", 11000);

            IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Byte[] recieveBytes = udpClientA.Receive(ref RemoteIPEndPoint);
            string returnData = Encoding.ASCII.GetString(recieveBytes);

            Console.WriteLine("This is the message you received " +
                                        returnData.ToString());
            Console.WriteLine("This message was sent from " +
                                        RemoteIPEndPoint.Address.ToString() +
                                        " on their port number " +
                                        RemoteIPEndPoint.Port.ToString());

            udpClientA.Close();
            udpClientB.Close();
        } 
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    
}

