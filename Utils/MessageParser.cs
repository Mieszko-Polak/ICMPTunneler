using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpPcap;

namespace ICMPTunneler.Utils;

class MessageParser()
{

    private readonly int maxMessageLength = 5200;
    private readonly int fragmentSize = 52;
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
            fragments[i] = ">" + fragmentNum + fragments[i] + "<";
        }
        return fragments;
    }

    public string Decode(string[] fragments)
    {
        fragments = fragments.Select(x => x[3..(x.Length-1)]).ToArray();
        return String.Join("",fragments);
    }

    public static (int, string) ExtractMessage(RawCapture rawPacket)
    {
        int fragmentNumber = -2;
        //var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
        string data = Encoding.ASCII.GetString(rawPacket.Data);
        if (data.Contains('>'))
        {
            int messageStart = data.IndexOf(">");
            if (data.Contains('<'))
            {
                int messageEnd = data.IndexOf("<");
                if(messageStart < data.Length - 3)
                {
                    try
                    {
                        fragmentNumber = Int32.Parse(data[(messageStart + 1)..(messageStart + 3)]);
                        return (fragmentNumber, data[(messageStart + 3)..messageEnd]);

                    } catch (FormatException)
                    {
                        if(data[messageStart+1] == 'H')
                        {
                            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                            var ip = packet.Extract<PacketDotNet.IPPacket>();
                            var sourceAddress = ip.SourceAddress;
                            return (-1, sourceAddress.ToString() + "@" + data[(messageStart + 2)..messageEnd]);
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
        if (data.Contains('>'))
        {
            int messageStart = data.IndexOf(">");
            if (messageStart < data.Length - 6)
            {
                if (data[messageStart..(messageStart + 7)] == ">|ACK|<")
                {
                    return true;
                }
            }
        }
        return false;
    }
}

            
            