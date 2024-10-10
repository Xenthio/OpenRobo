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
}
