using System.Text.RegularExpressions;

namespace OpenRobo.Utils;

internal class TimeUtil
{
	/// <summary>
	/// Time unit definitions in seconds.
	/// </summary>
	public static class TimeUnit
	{
		/// <summary>
		/// how many seconds in a second
		/// </summary>
		public const int Second = 1;
		/// <summary>
		/// how many seconds in a minute
		/// </summary>
		public const int Minute = Second * 60;
		/// <summary>
		/// how many minutes in an hour
		/// </summary>
		public const int Hour = Minute * 60;
		/// <summary>
		/// how many hours in a day
		/// </summary>
		public const int Day = Hour * 24;


		/// <summary>
		/// how many days in our definition of a week
		/// </summary>
		public const int Week = Day * 7;
		/// <summary>
		/// how many days in our definition of a month
		/// </summary>
		public const int Month = Day * 30;
		/// <summary>
		/// how many days in our definition of a year
		/// </summary>
		public const int Year = Day * 365;
	}
	static Regex NumbersOnlyRegex = new Regex("[0-9]+", RegexOptions.IgnoreCase);
	public static long ParseStringToSeconds(string time)
	{
		var timeint = long.Parse(NumbersOnlyRegex.Match(time).Value);
		if (new string[] { "s", "second", "seconds" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Second;
		if (new string[] { "m", "minute", "minutes" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Minute;
		if (new string[] { "h", "hour", "hours" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Hour;
		if (new string[] { "d", "day", "days" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Day;
		if (new string[] { "w", "week", "weeks" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Week;
		if (new string[] { "mn", "month", "months" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Month;
		if (new string[] { "y", "year", "years" }.Any(x => time.Contains(x))) timeint *= TimeUnit.Year;
		return timeint;
	}
	public static string ParseStringToString(string time)
	{
		var timeint = long.Parse(NumbersOnlyRegex.Match(time).Value);
		var s = (timeint == 1 || timeint == -1) ? "" : "s";
		var timestring = $"{timeint} second{s}";
		if (new string[] { "s", "second", "seconds" }.Any(x => time.Contains(x))) timestring = $"{timeint} second{s}";
		if (new string[] { "m", "minute", "minutes" }.Any(x => time.Contains(x))) timestring = $"{timeint} minute{s}";
		if (new string[] { "h", "hour", "hours" }.Any(x => time.Contains(x))) timestring = $"{timeint} hour{s}";
		if (new string[] { "d", "day", "days" }.Any(x => time.Contains(x))) timestring = $"{timeint} day{s}";
		if (new string[] { "w", "week", "weeks" }.Any(x => time.Contains(x))) timestring = $"{timeint} week{s}";
		if (new string[] { "mn", "month", "months" }.Any(x => time.Contains(x))) timestring = $"{timeint} month{s}";
		if (new string[] { "y", "year", "years" }.Any(x => time.Contains(x))) timestring = $"{timeint} year{s}";
		return timestring;
	}
	public static long TimeNow()
	{
		return DateTime.Now.Ticks;
	}
	public static long TimeSecondsFromNow(long seconds)
	{
		return DateTime.Now.Ticks + (seconds * 10000000);
	}
}
