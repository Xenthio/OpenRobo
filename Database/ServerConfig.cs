using Discord.WebSocket;
using OpenRobo.Commands;
using System.Reflection;

namespace OpenRobo.Database;

public class ServerConfig
{
	public ulong LevellingAnnouncementChannel { get; set; } = 0;
	public ulong DeletionLogChannel { get; set; } = 0;
	public ulong EditLogChannel { get; set; } = 0;
	public ulong DebugLogChannel { get; set; } = 0;
	public ulong MuteRole { get; set; } = 0;
	public ulong ImageMuteRole { get; set; } = 0;
	public ulong ReactMuteRole { get; set; } = 0;
	public List<ulong> SpamFilteredChannels { get; set; } = new();
	public Dictionary<int, ulong> LevellingRoleRewards { get; set; } = new();

	[ChatCommand("setsetting")]
	public static void SetSetting(SocketMessage socketMessage)
	{
		var cmdparams = socketMessage.Content.Split(" ");

		var settingname = cmdparams[1];
		var value = cmdparams[2];

		var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
		var guilduser = (socketMessage.Author as SocketGuildUser);
		if (guilduser.GuildPermissions.Administrator && typeof(ServerConfig).GetProperty(settingname) is PropertyInfo property)
		{
			var db = ServerInstance.GetOrCreateServerInstance(guild);
			var oldvalue = property.GetValue(db.Config);
			var newvalue = Convert.ChangeType(value, property.PropertyType);
			property.SetValue(db.Config, newvalue);
			socketMessage.Channel.SendMessageAsync($"Changed {settingname} from {oldvalue} to {newvalue}");
			db.SaveServerConfig();
		}
		else
		{
			socketMessage.Channel.SendMessageAsync($"Cannot change setting {settingname}");
		}
	}
}

