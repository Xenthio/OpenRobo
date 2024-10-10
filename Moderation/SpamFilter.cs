using Discord.WebSocket;
using OpenRobo.Database;
using OpenRobo.Utils;
using static OpenRobo.CommandSystem;

namespace OpenRobo.Moderation;

internal class SpamFilter
{
	[Events.MessageRecieved]
	public static void MessageRecieved(SocketMessage socketMessage)
	{
	}
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
