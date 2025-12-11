namespace ICMPTunneler.Utils;

class StringPadder()
{
    public static string PadToTwoDigits(string num)
    {
        if (num.Length < 2)
        {
            num = "0" + num;
        }
        return num;
    }
}
