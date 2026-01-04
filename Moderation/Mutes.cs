using Discord.WebSocket;
using OpenRobo.Commands;
using OpenRobo.Database;
using OpenRobo.Utils;

namespace OpenRobo.Moderation;

internal class Mutes
{
	public enum MuteType
	{
		ChatMute,
		ReactMute,
		ImageMute,
	}
	[Events.Tick]
	public static void DoMuteTick()
	{
		// TODO: On a server instance's initial load, we should maybe load muted users once? this is sorta fine for now
		foreach (var server in ServerInstance.Instances)
		{
			foreach (var muteduser in server.Database.Users.Where(x => x.IsMuted && TimeUtil.TimeNow() >= x.MutedUntil).ToList())
			{
				server.Server.GetUser(muteduser.ID).UnMute();
			}
			foreach (var muteduser in server.Database.Users.Where(x => x.IsReactMuted && TimeUtil.TimeNow() >= x.ReactMutedUntil).ToList())
			{
				server.Server.GetUser(muteduser.ID).UnReactMute();
			}
			foreach (var muteduser in server.Database.Users.Where(x => x.IsImageMuted && TimeUtil.TimeNow() >= x.ImageMutedUntil).ToList())
			{
				server.Server.GetUser(muteduser.ID).UnImageMute();
			}
		}
	}

	public static void SetUserMuted(SocketGuildUser user, MuteType type, bool mute, long time = 0)
	{
		var guild = user.Guild;
		var db = ServerInstance.GetOrCreateServerInstance(guild);
		var dbuser = db.GetOrRegisterUser(user);

		switch (type)
		{
			case MuteType.ChatMute:
				if (guild.GetRole(db.Config.MuteRole) == null) return;
				if (mute) user.AddRoleAsync(guild.GetRole(db.Config.MuteRole));
				else user.RemoveRoleAsync(guild.GetRole(db.Config.MuteRole));
				dbuser.IsMuted = mute;
				dbuser.MutedUntil = TimeUtil.TimeSecondsFromNow(time);
				break;
			case MuteType.ReactMute:
				if (guild.GetRole(db.Config.ReactMuteRole) == null) return;
				if (mute) user.AddRoleAsync(guild.GetRole(db.Config.ReactMuteRole));
				else user.RemoveRoleAsync(guild.GetRole(db.Config.ReactMuteRole));
				dbuser.IsReactMuted = mute;
				dbuser.ReactMutedUntil = TimeUtil.TimeSecondsFromNow(time);
				break;
			case MuteType.ImageMute:
				if (guild.GetRole(db.Config.ImageMuteRole) == null) return;
				if (mute) user.AddRoleAsync(guild.GetRole(db.Config.ImageMuteRole));
				else user.RemoveRoleAsync(guild.GetRole(db.Config.ImageMuteRole));
				dbuser.IsImageMuted = mute;
				dbuser.ImageMutedUntil = TimeUtil.TimeSecondsFromNow(time);
				break;
			default:
				break;
		}


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
			var targetuser = guild.GetUser(CommandUtils.ParseToID(cmdparams[1]));

			// this is in seconds
			long time = 60;
			string timestring = "60 seconds";
			var reason = "No reason specified";

			if (cmdparams.Length > 2)
			{
				time = TimeUtil.ParseStringToSeconds(string.Join(" ", cmdparams.Skip(2)));
				timestring = TimeUtil.ParseStringToString(string.Join(" ", cmdparams.Skip(2)));
			}

			var db = ServerInstance.GetOrCreateServerInstance(guild);
			var dbuser = db.GetOrRegisterUser(targetuser);
			if (!dbuser.IsMuted)
			{
				socketMessage.Channel.SendMessageAsync($"{targetuser.Username} has been muted for {timestring}.");
			}
			else
			{
				socketMessage.Channel.SendMessageAsync($"{targetuser.Username} mute changed to {timestring}.");
			}

			targetuser.Mute(time);

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
			var targetuser = guild.GetUser(CommandUtils.ParseToID(cmdparams[1]));

			targetuser.UnMute();
			socketMessage.Channel.SendMessageAsync($"{targetuser.Username} unmuted.");

		}
	}

	[ChatCommand("setmuterole")]
	public static void SetMutedRole(SocketMessage socketMessage)
	{
		var cmdparams = socketMessage.Content.Split(" ");

		var role = CommandUtils.ParseToID(cmdparams.Last());

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
