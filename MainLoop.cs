namespace OpenRobo;

internal class MainLoop
{
	internal static Task StartLoop()
	{
		if (MainGlobal.Client.Guilds.Count() >= 1) // check if bot is in a server
		{
			Console.WriteLine("| Servers detected: " + MainGlobal.Client.Guilds.Count());
		}
		else
		{
			Console.WriteLine($"| A server has not yet been defined.");
		}
		while (MainGlobal.Client.ConnectionState == Discord.ConnectionState.Connecting)
		{
			Console.WriteLine("| Waiting for connection to be established by Discord...");
			Task.Delay(1200);
		}
		return Task.CompletedTask;
	}
}
