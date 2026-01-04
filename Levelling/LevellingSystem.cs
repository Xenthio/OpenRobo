using Discord;
using Discord.WebSocket;
using OpenRobo.Commands;
using OpenRobo.Database;
using OpenRobo.Utils;

namespace OpenRobo.Levelling
{
	internal class LevellingSystem
	{
		[Events.MessageRecieved]
		public static void MessageRecieved(SocketMessage socketMessage)
		{
			if (socketMessage.Author.IsWebhook) return;
			if (socketMessage.Author.IsBot) return;
			// Check if we should give Xp
			if (!IsOnXPCooldown(socketMessage.Channel.Id, socketMessage.Author.Id))
			{
				GiveXP((socketMessage.Channel as SocketGuildChannel).Guild, socketMessage.Author, Random.Shared.Next(15, 25));
				DoXPCooldown(socketMessage.Channel.Id, socketMessage.Author.Id, 60);
			}
		}

		internal static HashSet<(ulong serverID, ulong userID)> MessageCooldowns = new();
		public static async Task DoXPCooldown(ulong serverID, ulong userID, int cdTime)
		{
			MessageCooldowns.Add((serverID, userID));
			await Task.Delay(cdTime * 1000);
			MessageCooldowns.Remove((serverID, userID));
		}

		public static bool IsOnXPCooldown(ulong serverID, ulong userID)
		{
			return MessageCooldowns.Contains((serverID, userID));
		}

		public static int XpRequiredForLevelUp(int level)
		{
			return 5 * (level * level) + (50 * level) + 100;
		}
		public static int TotalXpRequiredForLevel(int level)
		{
			return (int)(5f / 6f * level * (2f * level * level + 27f * level + 91f));
		}

		public static void GiveXP(SocketGuild server, SocketUser user, int xp)
		{
			var serverInstance = ServerInstance.GetOrCreateServerInstance(server);
			var userdata = serverInstance.GetOrRegisterUser(user);

			userdata.XP += xp;

			var nextlevel = userdata.Level + 1;
			// Check if we should level up after giving Xp. MEE6 level algorithm, source: https://github.com/Mee6/Mee6-documentation/blob/master/docs/levels_xp.md
			// (5f / 6f * nextlevel * (2f * nextlevel * nextlevel + 27f * nextlevel + 91f) also exists
			if (userdata.XP >= XpRequiredForLevelUp(userdata.Level))
			{
				userdata.Level += 1;
				userdata.XP = 0;

				// If level announcements are setup, do them.
				if (serverInstance.Config.LevellingAnnouncementChannel != 0)
				{
					if (MainGlobal.Client.GetChannel(serverInstance.Config.LevellingAnnouncementChannel) is IMessageChannel chn)
					{
						chn.SendMessageAsync($"<@{user.Id}> has leveled up to {userdata.Level}");
					}
				}

				// check if leveling up means we should assign a role.
				if (user is SocketGuildUser guildUser)
				{
					foreach (var reward in serverInstance.Config.LevellingRoleRewards)
					{
						if (userdata.Level >= reward.Key) guildUser.AddRoleAsync(server.GetRole(reward.Value));
					}
				}
			}
			serverInstance.SaveChanges();
		}
		public static void TakeXP(SocketGuild server, SocketUser user, int xp)
		{
			var serverInstance = ServerInstance.GetOrCreateServerInstance(server);
			var userdata = serverInstance.GetOrRegisterUser(user);

			userdata.XP -= xp;
			serverInstance.SaveChanges();
		}
		public static void SetXP(SocketGuild server, SocketUser user, int xp)
		{
			var serverInstance = ServerInstance.GetOrCreateServerInstance(server);
			var userdata = serverInstance.GetOrRegisterUser(user);

			userdata.XP = xp;
			serverInstance.SaveChanges();
		}

		public static void SetLevel(SocketGuild server, SocketUser user, int level)
		{
			var serverInstance = ServerInstance.GetOrCreateServerInstance(server);
			var userdata = serverInstance.GetOrRegisterUser(user);

			userdata.Level = level;
			serverInstance.SaveChanges();

			// Update user's roles to match their level
			if (user is SocketGuildUser guildUser)
			{
				// Remove roles that are no longer applicable
				var rolesToRemove = guildUser.Roles
				.Where(role => serverInstance.Config.LevellingRoleRewards.Values.Contains(role.Id) &&
				   serverInstance.Config.LevellingRoleRewards.Any(reward => reward.Key > level && reward.Value == role.Id))
				.ToList();

				foreach (var role in rolesToRemove)
				{
					guildUser.RemoveRoleAsync(role);
				}

				// Add roles that are now applicable
				foreach (var reward in serverInstance.Config.LevellingRoleRewards)
				{
					if (level >= reward.Key)
					{
						var role = server.GetRole(reward.Value);
						if (role != null && !guildUser.Roles.Any(r => r.Id == role.Id))
						{
							guildUser.AddRoleAsync(role);
						}
					}
				}
			}
		}

		[ChatCommand("rank")]
		public static void CheckLevel(SocketMessage socketMessage)
		{
			var db = ServerInstance.GetOrCreateServerInstance((socketMessage.Channel as SocketGuildChannel).Guild);
			var userdata = db.GetOrRegisterUser(socketMessage.Author);
			var nextlvl = XpRequiredForLevelUp(userdata.Level);
			var needed = nextlvl - userdata.XP;
			var totalxp = TotalXpRequiredForLevel(userdata.Level) + userdata.XP;
			socketMessage.Channel.SendMessageAsync($"You are level {userdata.Level}, you have {userdata.XP} Xp. You need {needed} more to level up. ({userdata.XP}/{nextlvl}). In total, you have {totalxp} Xp.");
		}

		[ChatCommand("setxp")]
		public static void SetXPCommand(SocketMessage socketMessage)
		{
			var cmdparams = socketMessage.Content.Split(" ");

			var xp = int.Parse(cmdparams.Last());

			var guilduser = (socketMessage.Author as SocketGuildUser);
			var guild = guilduser.Guild;
			if (guilduser.GuildPermissions.Administrator)
			{
				var targetuser = guild.GetUser(CommandUtils.ParseToID(cmdparams[1]));
				SetXP(guild, targetuser, xp);
				socketMessage.Channel.SendMessageAsync($"Set the xp of {targetuser.Id} to {xp}");
			}
		}
		[ChatCommand("setlevel")]
		public static void SetLevelCommand(SocketMessage socketMessage)
		{
			var cmdparams = socketMessage.Content.Split(" ");

			var level = int.Parse(cmdparams.Last());

			var guilduser = (socketMessage.Author as SocketGuildUser);
			var guild = guilduser.Guild;
			if (guilduser.GuildPermissions.Administrator)
			{
				var targetuser = guild.GetUser(CommandUtils.ParseToID(cmdparams[1]));
				SetLevel(guild, targetuser, level);
				socketMessage.Channel.SendMessageAsync($"Set the level of {targetuser.Id} to {level}");
			}
		}

		[ChatCommand("setlvlannounce")]
		public static void SetLevelingAnnouncementChannel(SocketMessage socketMessage)
		{
			var chnlID = socketMessage.Content.Split(" ").Last();
			var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
			var guilduser = (socketMessage.Author as SocketGuildUser);
			if (guilduser.GuildPermissions.Administrator)
			{
				var db = ServerInstance.GetOrCreateServerInstance(guild);
				db.Config.LevellingAnnouncementChannel = ulong.Parse(chnlID);
				socketMessage.Channel.SendMessageAsync($"Set level announcement channel to <#{chnlID}>");
				db.SaveServerConfig();
			}
		}
		[ChatCommand("setlvlrole")]
		public static void SetLevelingRoleReward(SocketMessage socketMessage)
		{
			var cmdparams = socketMessage.Content.Split(" ");

			var role = CommandUtils.ParseToID(cmdparams.Last());
			var level = int.Parse(cmdparams[1]);

			var guild = (socketMessage.Channel as SocketGuildChannel).Guild;
			var guilduser = (socketMessage.Author as SocketGuildUser);
			if (guilduser.GuildPermissions.Administrator)
			{
				var db = ServerInstance.GetOrCreateServerInstance(guild);
				db.Config.LevellingRoleRewards[level] = role;
				socketMessage.Channel.SendMessageAsync($"Set level {level} reward to <@&{role}>");
				db.SaveServerConfig();
			}
		}
	}
}
