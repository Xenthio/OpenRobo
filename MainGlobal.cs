
using Discord;
using Discord.WebSocket;
namespace OpenRobo;

public class MainGlobal
{
    private static System.Timers.Timer Loop;
    internal static SocketGuild Server { get; set; }
    internal static DiscordSocketClient Client { get; set; }
    internal static Discord.Rest.RestApplication ApplicationInfo { get; set; }
    internal static async Task AsyncWebUI(string[] args)
    {
        AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        {
            Exception ex = eventArgs.Exception;
            Log.Exception(ex, "WEBI", "44;1m");
        };
        OpenRobo.WebUI.MainWebUI.WebUIMain(args);
    }
    internal static async Task AsyncMain()
    {
        AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        {
            Exception ex = eventArgs.Exception;
            Log.Exception(ex, "DISC", "45;1m");
        };
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

        Settings.LoadOrCreateGlobalConfig();

        // no longer seems to work - used to detect user joining VC and remove their deny send msg perm to the vc chat text channel
        // Client.UserVoiceStateUpdated += Events.UserVoiceStateUpdated;
        await Client.LoginAsync(TokenType.Bot, Settings.Config.Token);

        await Client.StartAsync();

        ApplicationInfo = await Client.GetApplicationInfoAsync();

        Console.WriteLine($"|{DateTime.Now} | Starting tick loop...");
        Loop = new System.Timers.Timer()
        {
            Interval = 5500,
            AutoReset = true,
            Enabled = true
        };
        Loop.Elapsed += Events.Tick;

        // Block this task until the program is closed.
        await Task.Delay(-1);
        Log.Info("Shutting down.");
        Database.ServerInstance.SaveAll();
    }
}