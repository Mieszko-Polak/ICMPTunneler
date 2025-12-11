using System.Runtime.InteropServices;
using System.Text;
using ICMPTunneler.Utils;
using PacketDotNet;
using SharpPcap;

namespace ICMPTunneler.Services;

public class Sender
{
    public static async Task SendMessage(string destination, string message)
    {
        MessageParser parser = new();
        string[] fragments = parser.Encode(message);
        await Pinger.SendPing(destination, ">~|H" + fragments.Length.ToString() + "|~<");

        ListenForAck();

        Console.WriteLine("Got Here");
        foreach (string fragment in fragments)
        {
            Console.WriteLine(fragment);
            await Pinger.SendPing(destination, fragment);
        }
    }

    private static void ListenForAck()
    {
        var device = CaptureDeviceList.Instance[7];

        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            if (MessageParser.IsAck(rawPacket))
            {
                device.StopCapture();
            }
        }

        
        Console.WriteLine(device);
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.StartCapture();
    }
}