using System.Text.RegularExpressions;

namespace OpenRobo.Utils;

internal class CommandUtils
{

	static Regex IDRegex = new Regex("[0-9]+", RegexOptions.IgnoreCase);
	public static ulong ParseToUserID(string str)
	{
		if (IDRegex.IsMatch(str)) return ulong.Parse(IDRegex.Match(str).Value);
		return ulong.Parse(str);
	}
	public static ulong ParseToRoleID(string str)
	{
		if (IDRegex.IsMatch(str)) return ulong.Parse(IDRegex.Match(str).Value);
		return ulong.Parse(str);
	}
	public static ulong ParseToChannelID(string str)
	{
		if (IDRegex.IsMatch(str)) return ulong.Parse(IDRegex.Match(str).Value);
		return ulong.Parse(str);
	}
	public static bool TryParseToBool(string str, out bool boolout)
	{
		if (str.ToUpper() == "ON") { boolout = true; return true; }
		if (str.ToUpper() == "OFF") { boolout = false; return true; }
		if (str.ToUpper() == "TRUE") { boolout = true; return true; }
		if (str.ToUpper() == "FALSE") { boolout = false; return true; }
		if (str.ToUpper() == "1") { boolout = true; return true; }
		if (str.ToUpper() == "0") { boolout = false; return true; }
		boolout = false;
		return false;
	}
}
