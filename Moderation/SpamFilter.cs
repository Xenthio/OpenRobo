using Discord.WebSocket;
using OpenRobo.Database;
using OpenRobo.Utils;
using static OpenRobo.CommandSystem;

namespace OpenRobo.Moderation;

internal class SpamFilter
{
	public static Dictionary<ulong, int> UserSpamCounter = new();
	public static Dictionary<ulong, int> UserMuteCounter = new();
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
				CountCooldown(userid);
				if (UserSpamCounter.ContainsKey(userid) && UserSpamCounter[userid] > GetCooldownThresholdForUser(userid))
				{
					Mutes.MuteUser(user, GetCooldownTimeForUser(userid));

				}
			}
		}
	}
	public static async Task CountCooldown(ulong userid)
	{
		if (!UserSpamCounter.ContainsKey(userid))
			UserSpamCounter[userid] = 1;
		else
			UserSpamCounter[userid] += 1;
		Log.Verbose($"spam filter counter for {userid}: {UserSpamCounter[userid]}");
		await Task.Delay(2 * 1000);
		UserSpamCounter[userid] -= 1;
		if (UserSpamCounter[userid] <= 0) UserSpamCounter.Remove(userid);
	}

	public static int GetCooldownThresholdForUser(ulong userid)
	{
		// Todo: bias with user level
		return 5;
	}
	public static async Task CountMute(ulong userid)
	{
		if (!UserMuteCounter.ContainsKey(userid))
			UserMuteCounter[userid] = 1;
		else
			UserMuteCounter[userid] += 1;
		Log.Verbose($"spam filter mute counter for {userid}: {UserMuteCounter[userid]}");
		await Task.Delay(120 * 1000);
		UserMuteCounter[userid] -= 1;
		if (UserMuteCounter[userid] <= 0) UserMuteCounter.Remove(userid);
	}
	public static int GetCooldownTimeForUser(ulong userid)
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
