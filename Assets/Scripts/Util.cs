
public static class Util
{
    public static string ConvertSecondsToMinutesString(int totalSeconds)
    {
        // Calculate the minutes and seconds
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        // Format the result as a string "mm:ss"
        string timeString = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        return timeString;
    }
}
