using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpPcap;

namespace ICMPTunneler.Utils;

class MessageParser()
{

    private readonly int maxMessageLength = 4800;
    private readonly int fragmentSize = 48;
    public string[] Encode(string message)
    {
        if (message.Length > this.maxMessageLength)
        {
            throw new ArgumentException("Message too long");
        }
        if (message.Contains('<') || message.Contains('>'))
        {
            throw new ArgumentException("Message cannot contain triangle brackets");
        }
        var chunks = message.Chunk(this.fragmentSize);
        string[] fragments = chunks.ToArray().Select(x => new string(x)).ToArray();
        for(int i = 0; i < fragments.Length; i++)
        {
            string fragmentNum = i.ToString();
            if (fragmentNum.Length < 2)
            {
                fragmentNum = "0" + fragmentNum;
            } 
            fragments[i] = ">~|" + fragmentNum + fragments[i] + "|~<";
        }
        return fragments;
    }

    public string Decode(string[] fragments)
    {
        fragments = fragments.Select(x => x[5..(x.Length-3)]).ToArray();
        return String.Join("",fragments);
    }

    public static (int, string) ExtractMessage(RawCapture rawPacket)
    {
        int fragmentNumber = -2;
        //var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
        string data = Encoding.ASCII.GetString(rawPacket.Data);
        if (data.Contains('>'))
        {
            int messageStart = data.IndexOf(">~|");
            if (data.Contains(">~|"))
            {
                int messageEnd = data.IndexOf("|~<");
                if(messageStart < data.Length - 7)
                {
                    try
                    {
                        fragmentNumber = Int32.Parse(data[(messageStart + 3)..(messageStart + 5)]);
                        return (fragmentNumber, data[(messageStart + 5)..messageEnd]);

                    } catch (FormatException)
                    {
                        if(data[messageStart+3] == 'H')
                        {
                            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                            var ip = packet.Extract<PacketDotNet.IPPacket>();
                            var sourceAddress = ip.SourceAddress;
                            return (-1, sourceAddress.ToString() + "@" + data[(messageStart + 4)..messageEnd]);
                        }
                        return (-2, "");
                    }
                }
            }

        }
        return (-2, "");
    }

    public static bool IsAck(RawCapture rawPacket)
    {
        string data = Encoding.ASCII.GetString(rawPacket.Data);
        if (data.Contains(">~|"))
        {
            int messageStart = data.IndexOf(">~|");
            if (messageStart < data.Length - 8)
            {
                if (data[messageStart..(messageStart + 9)] == ">~|ACK|~<")
                {
                    return true;
                }
            }
        }
        return false;
    }
}

            
            