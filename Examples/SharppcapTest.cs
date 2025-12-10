using System.Runtime.CompilerServices;
using System.Text;
using SharpPcap;
using SharpPcap.LibPcap;

namespace ICMPTunneler.Examples;

class SharppcapTest
{
    public static void ListDevices()
    {
        var devices = CaptureDeviceList.Instance;
        foreach (var dev in devices)
        {
            Console.WriteLine("{0}\n", dev.ToString());
        }
    }

    public static async Task Listen()
    {
        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            var ip = packet.Extract<PacketDotNet.IPPacket>();
            byte[] bytePacket = e.Data.ToArray();
            Console.WriteLine(Encoding.ASCII.GetString(bytePacket));
        }

        var device = CaptureDeviceList.Instance[7];
        Console.WriteLine(device);
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.Capture();
    }

}