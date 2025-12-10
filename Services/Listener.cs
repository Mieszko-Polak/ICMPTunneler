using SharpPcap;
using ICMPTunneler.Utils;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using PacketDotNet;

namespace ICMPTunneler.Services;

public class Listener
{
    private bool currentlyCommunicating = false;
    private string communicatingIP = "";
    private string[] fragments = [];
    public void Listen()
    {
        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            var messageInfo = MessageParser.ExtractMessage(rawPacket);
            int fragmentNumber = messageInfo.Item1;
            string fragmentMessage = messageInfo.Item2;
            if (!currentlyCommunicating)
            {
                if(fragmentNumber == -1)
                {
                    var headerInfo = fragmentMessage.Split("@");
                    communicatingIP = headerInfo[0];
                    Console.WriteLine(communicatingIP + headerInfo[1]);
                    Array.Resize(ref fragments, Int32.Parse(headerInfo[1]));
                    currentlyCommunicating = true; 
                    Console.Write("Recieved");
                    Pinger.SendPing(communicatingIP, ">|ACK|<");
                    Console.WriteLine("Sent to " + communicatingIP);
                }
            } 
            else
            {
                if(fragmentNumber >= 0)
                {
                    Console.WriteLine("Recieved something");
                    fragments[fragmentNumber] = fragmentMessage;
                    foreach(string fragment in fragments)
                    {
                        Console.WriteLine(fragmentNumber);
                    }
                }
                if (!fragments.Any(string.IsNullOrEmpty))
                {
                    Console.WriteLine(String.Join("",fragments));
                    fragments = [];
                    currentlyCommunicating = false;
                    communicatingIP = "";
                }
            }

        }

        var device = CaptureDeviceList.Instance[7];
        Console.WriteLine(device);
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.Capture();
    }
}