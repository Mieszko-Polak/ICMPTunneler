using ICMPTunneler.Utils;
using SharpPcap;

namespace ICMPTunneler.Services;

public class ErrorCatcher
{
    public static async Task<List<int>> CatchErrors(List<int> sequenceNumbersToBeAcked, int timeout)
    {
        //Create an array to track which sequeces have been acknowedlged, and two tasks
        //One task to track if all sequence positions have been acknowledged, and one to keep track of the time set for this method
        bool[] ackedSequencePositions = new bool[sequenceNumbersToBeAcked.Count];
        var tcs = new TaskCompletionSource();
        var timeoutTask = Task.Delay(timeout);

        //Capture device setting - should be set independently for each computer
        var device = CaptureDeviceList.Instance[6];
        void Device_OnPacketArrival(object s, PacketCapture e)
        {
            //Read the packet, and if it is an ACK packet, mark it as acknowledged in the array
            var rawPacket = e.GetPacket();
            if (MessageParser.GetFlag(rawPacket).Item1 == "ACK")
            {
                if (sequenceNumbersToBeAcked.Contains(MessageParser.GetFlag(rawPacket).Item2))
                {
                    ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(MessageParser.GetFlag(rawPacket).Item2)] = true;
                }
            }
            //Success if all positions in array are true
            if (ackedSequencePositions.All(x => x))
            {
                tcs.TrySetResult();
            }
        }

        //Start capturing packets, with the above method happening on arriving
        device.Open();
        device.OnPacketArrival += Device_OnPacketArrival;
        device.StartCapture();

        //Wait for either success, or timeout
        var endErrorCatching = await Task.WhenAny(tcs.Task, timeoutTask);

        device.StopCapture();
        device.Close();
        
        //Return all fragment numbers which havent been Acked yet.
        return sequenceNumbersToBeAcked.Where(x => !ackedSequencePositions[sequenceNumbersToBeAcked.IndexOf(x)]).ToList();
    }
}