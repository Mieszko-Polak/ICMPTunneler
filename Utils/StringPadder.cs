namespace ICMPTunneler.Utils;

class StringPadder()
{
    //Pad a single digit number n to be "0n" as a string.
    public static string PadToTwoDigits(string num)
    {
        if (num.Length < 2)
        {
            num = "0" + num;
        }
        return num;
    }
}
