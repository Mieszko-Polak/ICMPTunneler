// See https://aka.ms/new-console-template for more information
using System.Net;
using ICMPTunneler.Examples;
using ICMPTunneler.Services;
using ICMPTunneler.Utils;

//await PingTest.SendPing();
//await IcmpSocketTest.SendIcmpTest(IPAddress.Parse("192.168.1.76"));

//await SharppcapTest.Listen();

string destinationIp = "127.0.0.1";
Console.Write("Listen (L), Send (S) or view available capture devices (D)");
var choice = Console.ReadLine();
if (choice == "L")
{
    Listener listener = new();
    listener.Listen();
}
else if (choice == "S")
{
    while (true)
    {
        Console.WriteLine("Input message to send: ");
        string message = Console.ReadLine();
        var chunks = message.Chunk(4000);
        foreach (char[] chunk in chunks)
        {
            await Sender.SendMessage(destinationIp, new string(chunk));
            await Task.Delay(1000);
        }
    }
} 
else
{
    SharppcapTest.ListDevices();
}

/*
MessageParser parser = new();
string[] fragments = parser.Encode("Ok so this is going to have to be at least 52 characters, I wonder if it will work when I type a fucking essay in this box");
foreach (string fragment in fragments)
{
    Console.WriteLine(fragment);
}
Console.WriteLine(parser.Decode(fragments));
*/
