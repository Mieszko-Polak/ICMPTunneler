# ICMPTunneler
Project for Micro-Internship at Modux.


Sequence, for n fragments is:
-1 -> Synchronisation Packet
[0,..,n] -> fragments
n+1 -> Fin packet

Things to set:
 -> In Program.cs, set destinationIP
 -> In Services/Listen.cs and Services/ErrorCatcher.cs, set the Capture Device appropriately (run, and input D to see device choices, then edit index appropriately)