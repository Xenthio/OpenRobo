namespace OpenRobo;

internal class Program
{
	public static async Task Main(string[] args)
	{

		AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
		{
			Exception ex = eventArgs.Exception;
			Log.Exception(ex, "DISC", "45;1m");
		};
		Console.WriteLine($"Loaded OpenRobo Test!");
		Console.WriteLine($"|{DateTime.Now} | Starting RoboSinc...");
		Task.Run(() => MainGlobal.AsyncMain());
		Console.WriteLine($"|{DateTime.Now} | Starting RoboSinc WebUI...");
		Task.Run(() => MainGlobal.AsyncWebUI(args));
		await Task.Delay(-1);
	}
}
