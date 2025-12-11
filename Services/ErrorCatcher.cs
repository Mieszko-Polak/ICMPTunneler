using ICMPTunneler.Utils;
using SharpPcap;

namespace ICMPTunneler.Services;

public class ErrorCatcher
{
    public static async Task<List<int>> CatchErrors(List<int> sequenceNumbersToBeAcked, int timeout)
    {
        bool[] ackedSequencePositions = new bool[sequenceNumbersToBeAcked.Count];
        var tcs = new TaskCompletionSource();
        var timeoutTask = Task.Delay(timeout);

        //Console.WriteLine(sequenceNumbersToBeAcked[0]);
        var device = CaptureDeviceList.Instance[1];
        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            //Console.WriteLine("GotHer");
            var rawPacket = e.GetPacket();
            if (MessageParser.GetFlag(rawPacket).Item1 == "ACK")
            {
                if (sequenceNumbersToBeAcked.Contains(MessageParser.GetFlag(rawPacket).Item2))
                {
                    //Console.WriteLine(MessageParser.GetFlag(rawPacket).Item2);
                    ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(MessageParser.GetFlag(rawPacket).Item2)] = true;
                }
            }
            if (ackedSequencePositions.All(x => x))
            {
                //Console.WriteLine("All Acknowledged");
                tcs.TrySetResult();
            }
        }

        //Console.WriteLine("Got Here");
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.StartCapture();

        var endErrorCatching = await Task.WhenAny(tcs.Task, timeoutTask);

        device.StopCapture();
        device.OnPacketArrival -= Device_OnPacketArrival;

        //Return the unacknloweged list of packets
        //Console.WriteLine(String.Join(",", sequenceNumbersToBeAcked.Where(x => !ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(x)]).ToList()));
        return sequenceNumbersToBeAcked.Where(x => !ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(x)]).ToList();
    }
}