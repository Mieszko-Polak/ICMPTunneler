using System.Runtime.InteropServices;
using System.Text;
using ICMPTunneler.Utils;
using PacketDotNet;
using SharpPcap;

namespace ICMPTunneler.Services;

public class Sender
{}
    public static async Task SendMessage(string destination, string message)
    {
        MessageParser parser = new();
        string[] fragments = parser.Encode(message);

        await Pinger.SendPing(destination, ">~|SYN" + StringPadder.PadToTwoDigits(fragments.Length.ToString()) + "|~<");

        ListenForAckToSyn();

        Console.WriteLine("Got Here");
        foreach (string fragment in fragments)
        {
            Console.WriteLine(fragment);
            Pinger.SendPing(destination, fragment);
        }

        CatchErrors(fragments);
    }

    private static void ListenForAckToSyn()
    {
        var device = CaptureDeviceList.Instance[7];

        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            if (MessageParser.GetFlag(rawPacket).Item1 == "ACK" && MessageParser.GetFlag(rawPacket).Item2 == -1)
            {
                device.StopCapture();
            }
        }

        
        Console.WriteLine(device);
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.StartCapture();
    }

    private static void CatchErrors(string[] fragments)
    {
        
    }
}