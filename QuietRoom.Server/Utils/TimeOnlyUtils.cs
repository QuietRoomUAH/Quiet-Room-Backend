using System.Text.RegularExpressions;

namespace QuietRoom.Server.Utils;

public static class TimeOnlyUtils
{
    public const string TIME_REGEX = @"^(\d\d)(\d\d)$";
    
    public static TimeOnly ParseTime(string time)
    {
        var match = Regex.Match(time, TIME_REGEX);
        var hour = int.Parse(match.Groups[1].Value);
        var minute = int.Parse(match.Groups[2].Value);
        return new TimeOnly(hour, minute);
    }

    /// <summary>
    /// Takes a time only and converts it into a very basic string
    /// </summary>
    /// <param name="time">The time to convert</param>
    /// <returns>The string in the format of HHmm</returns>
    public static string ToBasicString(this TimeOnly time)
    {
        return time.ToString("HHmm");
    }
}
