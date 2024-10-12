namespace OpenRobo;

internal class Program
{
    public static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        {
            Exception ex = eventArgs.Exception;
            Log.Exception(ex);
        };
        Console.WriteLine($"Loaded OpenRobo Test!");
        Console.WriteLine($"|{DateTime.Now} | Starting RoboSinc...");
        MainGlobal.AsyncMain();
        Console.WriteLine($"|{DateTime.Now} | Starting RoboSinc WebUI...");
        MainGlobal.AsyncWebUI(args);
        await Task.Delay(-1);
    }
}
