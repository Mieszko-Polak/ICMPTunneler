// See https://aka.ms/new-console-template for more information
using System.Net;
using ICMPTunneler.Examples;

await SocketTestLoop.SendToLocalhost();
await SocketTestLoop.SendToRouter4();
await SocketTestLoop.SendToRouter6();

Console.WriteLine("Done.");