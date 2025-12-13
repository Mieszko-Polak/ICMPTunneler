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
            //Logic for Synchonization packet, if communication hasn't already been established
            if(MessageParser.GetFlag(rawPacket).Item1 == "SYN" && !currentlyCommunicating)
            {
                //In this case, we need the IP to send back to
                var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                var ip = packet.Extract<PacketDotNet.IPPacket>();
                communicatingIP = ip.SourceAddress.ToString();
                //Resize the fragments array, so that it can accurately track how many packets have been recieved
                Array.Resize(ref fragments, MessageParser.GetFlag(rawPacket).Item2);
                currentlyCommunicating = true; 
                //Reply, acknowledging the synchronization packet has been processed
                Pinger.SendPing(communicatingIP, ">~|ACK-1|~<");
            } 
            //Logic for fragments of the message coming in after comunication has been established
            else if(currentlyCommunicating)
            {
                var messageInfo = MessageParser.ExtractMessage(rawPacket);
                int fragmentNumber = messageInfo.Item1;
                string fragmentMessage = messageInfo.Item2;
                //If its a packet that should be cared about, add its message to the array
                if(fragmentNumber >= 0)
                {
                    fragments[fragmentNumber] = fragmentMessage;
                    Pinger.SendPing(communicatingIP, ">~|ACK"+StringPadder.PadToTwoDigits(fragmentNumber.ToString())+"|~<");
                }
                //If all of the packets have come through, assemble the message, and output it
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