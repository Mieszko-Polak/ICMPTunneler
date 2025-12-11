// See https://aka.ms/new-console-template for more information
using System.Net;
using ICMPTunneler.Examples;
using ICMPTunneler.Services;
using ICMPTunneler.Utils;

//await PingTest.SendPing();
//await IcmpSocketTest.SendIcmpTest(IPAddress.Parse("192.168.1.76"));
SharppcapTest.ListDevices();
//await SharppcapTest.Listen();


Console.Write("Listen (L) or Send (S)");
var choice = Console.ReadLine();
if (choice == "L")
{
    Listener listener = new();
    listener.Listen();
}
else
{
    await Sender.SendMessage("127.0.0.1","Ok so this is going to have to be at least 52 characters, I wonder if it will work when I type an essay in this box. There is a max of 5200 characters I think, so I could pu tmy personal statement in here, and it owuld be fine. Maybe I'll make a file it can read from , and it can go from there?");
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
