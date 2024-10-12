using Discord;
using Discord.WebSocket;
using OpenRobo.Commands;
using OpenRobo.Database;

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
    public static void Error(string message, string realm = "UNKN", string colour = "101;1m")
    {
        Console.WriteLine($"\u001b[{colour}[  {realm}  ]\u001b[41;1m[  ERR  ]\u001b[0m MSG: {message}\n");
    }
    public static void Exception(Exception ex, string realm = "UNKN", string colour = "101;1m")
    {
        Console.WriteLine($"\u001b[{colour}[  {realm}  ]\u001b[41;1m[  ERR  ]\u001b[0m MSG: {ex.Message} \n WHERE: {ex.StackTrace} \n\n");
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


    public static void InServer(SocketGuild guild, string message)
    {
        var serverInstance = ServerInstance.GetOrCreateServerInstance(guild);
        if (serverInstance.Config.DebugLogChannel != 0)
        {
            var channel = guild.GetTextChannel(serverInstance.Config.DebugLogChannel);
            channel.SendMessageAsync(message);
        }
    }

    [ChatCommand("setdebuglog")]
    public static void SetDebugLogChannel(SocketMessage socketMessage)
    {
        var chnlID = socketMessage.Content.Split(" ").Last();
        var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
        var guilduser = (socketMessage.Author as SocketGuildUser);
        if (guilduser.GuildPermissions.Administrator)
        {
            var db = ServerInstance.GetOrCreateServerInstance(guild);
            db.Config.DebugLogChannel = ulong.Parse(chnlID);
            socketMessage.Channel.SendMessageAsync($"Set debug logging channel to <#{chnlID}>");
            db.SaveServerConfig();
        }
    }

}
