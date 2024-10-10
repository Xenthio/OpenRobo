using Discord.WebSocket;
using OpenRobo.Database;
using OpenRobo.Utils;
using static OpenRobo.CommandSystem;

namespace OpenRobo.Moderation;

internal class Mutes
{
	[Events.Tick]
	public static void DoMuteTick()
	{
		// TODO: On a server instance's initial load, we should maybe load muted users once? this is sorta fine for now
		foreach (var server in ServerInstance.Instances)
		{
			foreach (var muteduser in server.Database.Users.Where(x => x.IsMuted && TimeUtil.TimeNow() >= x.MutedUntil))
			{
				UnMuteUser(server.Server.GetUser(muteduser.ID));
			}
		}
	}
	public static void UnMuteUser(SocketGuildUser user)
	{
		var guild = user.Guild;
		var db = ServerInstance.GetOrCreateServerInstance(guild);
		var dbuser = db.GetOrRegisterUser(user);

		user.RemoveRoleAsync(guild.GetRole(db.Config.MuteRole));

		dbuser.IsMuted = false;

		db.SaveChanges();
	}
	public static void MuteUser(SocketGuildUser user, long time)
	{
		var guild = user.Guild;
		var db = ServerInstance.GetOrCreateServerInstance(guild);
		var dbuser = db.GetOrRegisterUser(user);

		user.AddRoleAsync(guild.GetRole(db.Config.MuteRole));

		dbuser.IsMuted = true;
		dbuser.MutedUntil = TimeUtil.TimeSecondsFromNow(time);

		db.SaveChanges();
	}

	[ChatCommand("mute")]
	public static void MuteCommand(SocketMessage socketMessage)
	{
		var cmdparams = socketMessage.Content.Split(" ");
		var authoruser = (socketMessage.Author as SocketGuildUser);
		if (authoruser.GuildPermissions.ModerateMembers)
		{
			var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
			var targetuser = guild.GetUser(CommandUtils.ParseToUserID(cmdparams[1]));

			// this is in seconds
			long time = 60;
			string timestring = "60 seconds";
			var reason = "No reason specified";

			if (cmdparams.Length > 2)
			{
				time = TimeUtil.ParseStringToSeconds(string.Join(" ", cmdparams.Skip(2)));
				timestring = TimeUtil.ParseStringToString(string.Join(" ", cmdparams.Skip(2)));
			}

			MuteUser(targetuser, time);
			socketMessage.Channel.SendMessageAsync($"{targetuser.Username} has been muted for {timestring}.");

		}
	}
	[ChatCommand("unmute")]
	public static void UnMuteCommand(SocketMessage socketMessage)
	{
		var cmdparams = socketMessage.Content.Split(" ");
		var authoruser = (socketMessage.Author as SocketGuildUser);
		if (authoruser.GuildPermissions.ModerateMembers)
		{
			var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
			var targetuser = guild.GetUser(CommandUtils.ParseToUserID(cmdparams[1]));

			UnMuteUser(targetuser);
			socketMessage.Channel.SendMessageAsync($"{targetuser.Username} unmuted.");

		}
	}

	[ChatCommand("setmuterole")]
	public static void SetMutedRole(SocketMessage socketMessage)
	{
		var cmdparams = socketMessage.Content.Split(" ");

		var role = CommandUtils.ParseToRoleID(cmdparams.Last());

		var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
		var guilduser = (socketMessage.Author as SocketGuildUser);
		if (guilduser.GuildPermissions.Administrator)
		{
			var db = ServerInstance.GetOrCreateServerInstance(guild);
			db.Config.MuteRole = role;
			socketMessage.Channel.SendMessageAsync($"Set muted role to <@&{role}>");
			db.SaveServerConfig();
		}
	}
}
