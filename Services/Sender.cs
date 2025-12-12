using System.Runtime.InteropServices;
using System.Text;
using ICMPTunneler.Utils;
using PacketDotNet;
using SharpPcap;
using System.Threading.Tasks;

namespace ICMPTunneler.Services;

public class Sender
{
    public static async Task SendMessage(string destination, string message)
    {
        MessageParser parser = new();
        string username = "User 2";
        string[] fragments = parser.Encode(username + ": " + message);

        await Pinger.SendPing(destination, ">~|SYN" + StringPadder.PadToTwoDigits(fragments.Length.ToString()) + "|~<");
        //Console.WriteLine(">~|SYN" + StringPadder.PadToTwoDigits(fragments.Length.ToString()) + "|~<");

        await ErrorCatcher.CatchErrors(new List<int> {-1}, 2000);

        List<int> fragmentsToSend = Enumerable.Range(0, fragments.Length).ToList();
        int sendTries = 0;
        while(sendTries < 100 && fragmentsToSend.Count != 0)
        {
            foreach (int fragmentNum in fragmentsToSend)
            {
                Pinger.SendPing(destination, fragments[fragmentNum], 4);
                //Console.WriteLine(fragmentNum);
            }
            fragmentsToSend = await ErrorCatcher.CatchErrors(fragmentsToSend, 2000);
            //Console.WriteLine(String.Join(", ", fragmentsToSend));
            //Console.WriteLine(fragmentsToSend.Count);
            sendTries++;
        }
        if (fragmentsToSend.Count != 0)
        {
            Console.Write("These fragments were not sent: ");
            foreach (int fragmentNum in fragmentsToSend)
            {
                Console.WriteLine(fragments[fragmentNum].ToString());
            }
        }
        else
        {
            Console.WriteLine("Message Delivered Successfully");
        }
    }
}