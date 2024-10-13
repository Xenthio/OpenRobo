using Discord;
using Discord.WebSocket;
using OpenRobo.Commands;
using OpenRobo.Database;
using OpenRobo.Utils;

namespace OpenRobo.Moderation;

internal class UserSpamOffenceTracker
{
	public int ChatSmall = 0;
	public int ChatMedium = 0;
	public int ChatLarge = 0;
	public int ChatHuge = 0;

	public int Embed = 0;
	public int React = 0;
	public int Ping = 0;

	public void AddCooldownPointForCategory(int category, int points)
	{
		if (category == 1) ChatSmall += points;
		if (category == 2) ChatMedium += points;
		if (category == 3) ChatLarge += points;
		if (category == 4) ChatHuge += points;
	}
	public bool ShouldMute()
	{
		if (ChatSmall >= 8) return true;
		if (ChatMedium >= 4) return true;
		if (ChatLarge >= 3) return true;
		if (ChatHuge >= 2) return true;

		if (Embed >= 8) return true;
		if (React >= 10) return true;
		if (Ping >= 9) return true;

		return false;
	}

	public bool ShouldDispose()
	{
		if (ChatSmall <= 0 &&
			ChatMedium <= 0 &&
			ChatLarge <= 0 &&
			ChatHuge <= 0 &&
			Embed <= 0 &&
			React <= 0 &&
			Ping <= 0) return true;
		return false;
	}
}
internal class SpamFilter
{
	public static Dictionary<ulong, UserSpamOffenceTracker> UserSpamOffences = new();
	[Events.MessageRecieved]
	public static void MessageRecieved(SocketMessage socketMessage)
	{
		if (socketMessage.Channel is SocketGuildChannel channel)
		{
			var guild = channel.Guild;
			var serverInstance = ServerInstance.GetOrCreateServerInstance(guild);
			if (serverInstance.Config.SpamFilteredChannels.Contains(socketMessage.Channel.Id) && socketMessage.Author is SocketGuildUser user)
			{
				var userid = socketMessage.Author.Id;
				DoMessageSpecificCooldown(userid, socketMessage.Content);
				if (UserSpamOffences.ContainsKey(userid) && UserSpamOffences[userid].ShouldMute())
				{
					var time = GetSpamMuteTimeForUser(userid);
					Log.InServer(guild, $"Gave a spam filter mute to <@{userid}> for {time} seconds.");
					Mutes.MuteUser(user, time);
				}
			}
		}
	}
	[Events.ReactionAdded]
	public static void ReactionAdded(Cacheable<IUserMessage, ulong> cachedUserMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction socketReaction)
	{
		if (cachedChannel.Value is SocketGuildChannel channel && cachedUserMessage.Value is SocketUserMessage msg && socketReaction.User.Value is SocketGuildUser user)
		{
			var guild = channel.Guild;
			var serverInstance = ServerInstance.GetOrCreateServerInstance(guild);
			if (serverInstance.Config.SpamFilteredChannels.Contains(channel.Id))
			{
				var userid = user.Id;
				DoReactCooldown(userid);
				if (UserSpamOffences.ContainsKey(userid) && UserSpamOffences[userid].ShouldMute())
				{
					var time = GetSpamMuteTimeForUser(userid);
					Log.InServer(guild, $"Gave a embed spam filter mute to <@{userid}> for {time} seconds.");
					Mutes.MuteUser(user, time);
				}
			}
		}
	}
	public static async Task DoMessageSpecificCooldown(ulong userid, string message)
	{
		if (!UserSpamOffences.ContainsKey(userid)) UserSpamOffences[userid] = new UserSpamOffenceTracker();
		GetCooldownCategoryForUserAndMessage(userid, message, out var category, out var time);
		UserSpamOffences[userid].AddCooldownPointForCategory(category, 1);
		await Task.Delay(time * 1000);
		UserSpamOffences[userid].AddCooldownPointForCategory(category, -1);
		if (UserSpamOffences[userid].ShouldDispose()) UserSpamOffences.Remove(userid);

	}
	public static async Task DoReactCooldown(ulong userid)
	{

	}

	public static void GetCooldownCategoryForUserAndMessage(ulong userid, string message, out int category, out int cooldowntime)
	{
		var lines = message.Split('\n').Count();
		var letters = message.Count();

		category = 0; cooldowntime = 0;

		if (lines <= 1) { category = 1; cooldowntime = 4; }
		if ((lines >= 2 && lines <= 3) || letters >= 180) { category = 2; cooldowntime = 18; }
		if ((lines >= 3 && lines <= 5) || letters >= 260) { category = 3; cooldowntime = 35; }
		if (lines >= 5 || letters >= 300) { category = 4; cooldowntime = 61; }
		// Todo: bias with user level 
	}
	public static int GetSpamMuteTimeForUser(ulong userid)
	{
		// Todo: bias with recent mutes in last 2 minutes (see function CountMute())
		return 30;
	}
	/// <summary>
	/// Toggle or view the state of the spam filter for this channel.
	/// </summary>
	/// <param name="socketMessage"></param>
	[ChatCommand("spamfilter")]
	public static void SpamFilterCommand(SocketMessage socketMessage)
	{
		var cmdparams = socketMessage.Content.Split(" ");
		var authoruser = (socketMessage.Author as SocketGuildUser);
		if (authoruser.GuildPermissions.ModerateMembers)
		{
			var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
			var chnlID = socketMessage.Channel.Id;
			var serverInstance = ServerInstance.GetOrCreateServerInstance(guild);
			if (cmdparams.Length <= 1)
			{
				socketMessage.Channel.SendMessageAsync($"This command sets the spam filter for the channel that you type it in. >spamfilter on/off will enable/disable the spam filter in this channel.\nSpam filter is set to {(serverInstance.Config.SpamFilteredChannels.Contains(chnlID) ? "on" : "off")} in this chat.");
				return;
			}

			if (CommandUtils.TryParseToBool(cmdparams[1], out var shouldEnable))
			{
				if (shouldEnable)
				{
					if (serverInstance.Config.SpamFilteredChannels.Contains(chnlID))
					{
						socketMessage.Channel.SendMessageAsync($"Spam filter already on.");
					}
					else
					{
						serverInstance.Config.SpamFilteredChannels.Add(chnlID);
						serverInstance.SaveServerConfig();
						socketMessage.Channel.SendMessageAsync($"This chat will now be spam filtered.");
					}
				}
				else
				{
					if (serverInstance.Config.SpamFilteredChannels.Contains(chnlID))
					{
						serverInstance.Config.SpamFilteredChannels.Remove(chnlID);
						serverInstance.SaveServerConfig();
						socketMessage.Channel.SendMessageAsync($"Spam filter disabled in this chat.");
					}
					else
					{
						socketMessage.Channel.SendMessageAsync($"Spam filter already off.");
					}
				}
			}

		}
	}
}
