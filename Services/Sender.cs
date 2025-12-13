using System.Runtime.InteropServices;
using System.Text;
using ICMPTunneler.Utils;
using PacketDotNet;
using SharpPcap;
using System.Threading.Tasks;

namespace ICMPTunneler.Services;

//Class responsible for sending packets, and gathering acknowledgements that packets have been send 
public class Sender
{
    public static async Task SendMessage(string destination, string message)
    {
        MessageParser parser = new();
        string username = "User";
        string[] fragments = parser.Encode(username + ": " + message);

        //Sending a Synchronization packet to start communication, along with how many fragments to accept
        await Pinger.SendPing(destination, ">~|SYN" + StringPadder.PadToTwoDigits(fragments.Length.ToString()) + "|~<");

        //-1 is the sequence number given to the SYN packet
        await ErrorCatcher.CatchErrors(new List<int> {-1}, 2000);

        //Creating a list tracking the indicies of fragments not yet acknowledged
        List<int> fragmentsToSend = Enumerable.Range(0, fragments.Length).ToList();
        int sendTries = 0;
        while(sendTries < 100 && fragmentsToSend.Count != 0)
        {
            //Sending not yet acknowledged fragments
            foreach (int fragmentNum in fragmentsToSend)
            {
                Pinger.SendPing(destination, fragments[fragmentNum], 4);
            }
            //Will return a new list of all fragments not yet acknowledged, 2 seconds after the ping
            fragmentsToSend = await ErrorCatcher.CatchErrors(fragmentsToSend, 2000);
            sendTries++;
        }
        // Show unsent fragments
        if (fragmentsToSend.Count != 0)
        {
            Console.Write("These fragments were not sent: ");
            foreach (int fragmentNum in fragmentsToSend)
            {
                Console.WriteLine(fragments[fragmentNum].ToString());
            }
        }
        //Or confirm that all fragments were sent
        else
        {
            Console.WriteLine("A chunk of the message was successfully delivered");
        }
    }
}