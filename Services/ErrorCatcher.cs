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
        var device = CaptureDeviceList.Instance[6];
        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            if (MessageParser.GetFlag(rawPacket).Item1 == "ACK")
            {
                if (sequenceNumbersToBeAcked.Contains(MessageParser.GetFlag(rawPacket).Item2))
                {
                    ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(MessageParser.GetFlag(rawPacket).Item2)] = true;
                }
            }
            if (ackedSequencePositions.All(x => x))
            {
                tcs.TrySetResult();
            }
        }

        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.StartCapture();

        var endErrorCatching = await Task.WhenAny(tcs.Task, timeoutTask);

        device.StopCapture();
        device.Close();
        
        return sequenceNumbersToBeAcked.Where(x => !ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(x)]).ToList();
    }
}