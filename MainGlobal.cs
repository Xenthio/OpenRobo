
using Discord;
using Discord.WebSocket;

using System.Timers;
namespace OpenRobo;

public class MainGlobal
{
	private static System.Timers.Timer Loop;
	internal static SocketGuild Server { get; set; }
	internal static DiscordSocketClient Client { get; set; }
	internal static Discord.Rest.RestApplication ApplicationInfo { get; set; }
	public static async Task Main()
	{
		Console.WriteLine($"Loaded OpenRobo Test!");
		Console.WriteLine($"|{DateTime.Now} | Starting RoboSinc...");
		AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
		{
			Exception ex = eventArgs.Exception;
			Log.Exception(ex);
		};
		await MainGlobal.AsyncMain();
	}
	private static async Task AsyncMain()
	{
		Console.WriteLine($"|{DateTime.Now} | Main loop initialised");
		Client = new DiscordSocketClient(new DiscordSocketConfig
		{
			MessageCacheSize = 1200,
			LogLevel = LogSeverity.Debug,
			AlwaysDownloadUsers = true,
			GatewayIntents =
			   GatewayIntents.MessageContent |
			   GatewayIntents.Guilds |
			   GatewayIntents.GuildMembers |
			   GatewayIntents.GuildPresences |
			   GatewayIntents.GuildMessageReactions |
			   GatewayIntents.GuildMessages |
			   GatewayIntents.GuildVoiceStates
		});
		Client.Log += Events.LogMessage;
		Client.Ready += MainLoop.StartLoop;
		Client.UserJoined += Events.UserJoined;
		Client.MessageUpdated += Events.MessageUpdated;
		Client.MessageReceived += Events.MessageRecieved;
		Client.MessageDeleted += Events.MessageDeleted;
		Client.ReactionAdded += Events.ReactionAdded;
		//Client.RoleCreated += Events.RoleCreated;
		//Client.GuildMemberUpdated += Events.GuildMemberUpdated;

		// no longer seems to work - used to detect user joining VC and remove their deny send msg perm to the vc chat text channel
		// Client.UserVoiceStateUpdated += Events.UserVoiceStateUpdated;
		await Client.LoginAsync(TokenType.Bot, Settings.Token);

		await Client.StartAsync();

		ApplicationInfo = await Client.GetApplicationInfoAsync();

		// Block this task until the program is closed.
		await Task.Delay(-1);
		Log.Info("Shutting down.");
		Database.ServerDatabase.SaveAll();
	}
	private static void Tick(object sender, ElapsedEventArgs e)
	{

	}
}