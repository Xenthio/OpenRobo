using Discord;

namespace OpenRobo;

public class Log
{
	public static bool VerboseLogging = true;
	private static readonly string[] logBlacklist = new string[]
	{
		"PRESENCE_UPDATE",
		"TYPING_START",
		"MESSAGE_CREATE",
		"MESSAGE_DELETE",
		"MESSAGE_UPDATE",
		"CHANNEL_UPDATE",
		"GUILD_",
		"REACTION_",
		"VOICE_STATE_UPDATE",
		"DELETE channels/",
		"POST channels/",
		"Heartbeat",
		"GET ",
		"PUT ",
		"Latency = ",
		"handler is blocking the"
	};
	public static void Error(string message)
	{
		Console.WriteLine($"\u001b[45;1m[  DISC  ]\u001b[41;1m[  ERR  ]\u001b[0m MSG: {message}\n");
	}
	public static void Exception(Exception ex)
	{
		Console.WriteLine($"\u001b[45;1m[  DISC  ]\u001b[41;1m[  ERR  ]\u001b[0m MSG: {ex.Message} \n WHERE: {ex.StackTrace} \n\n");
	}
	public static void Info(string message)
	{
		Console.WriteLine($"|{DateTime.Now} | {message}");
	}
	public static void Verbose(string message)
	{
		if (VerboseLogging)
			Console.WriteLine($"|{DateTime.Now} | Verbose | {message}");
	}
	public static void Message(LogMessage Msg)
	{
		if (Msg.Message != null && logBlacklist.All(x => !Msg.Message.Contains(x)))
			Console.WriteLine($"|{DateTime.Now} - {Msg.Source} | {Msg.Message}");
		else if (Msg.Exception != null)
			Console.WriteLine($"|{DateTime.Now} - {Msg.Source} | {Msg.Exception}");
	}
}
