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
            if(MessageParser.GetFlag(rawPacket).Item1 == "SYN" && !currentlyCommunicating)
            {
                var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                var ip = packet.Extract<PacketDotNet.IPPacket>();
                communicatingIP = ip.SourceAddress.ToString();
                Array.Resize(ref fragments, MessageParser.GetFlag(rawPacket).Item2);
                currentlyCommunicating = true; 
                Pinger.SendPing(communicatingIP, ">~|ACK-1|~<");
            } 
            else if(currentlyCommunicating)
            {
                var messageInfo = MessageParser.ExtractMessage(rawPacket);
                int fragmentNumber = messageInfo.Item1;
                string fragmentMessage = messageInfo.Item2;
                if(fragmentNumber >= 0)
                {
                    fragments[fragmentNumber] = fragmentMessage;
                    Pinger.SendPing(communicatingIP, ">~|ACK"+StringPadder.PadToTwoDigits(fragmentNumber.ToString())+"|~<");
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

        var device = CaptureDeviceList.Instance[6];


        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.Capture();
    }
}