using Discord.WebSocket;
using static OpenRobo.Moderation.Mutes;

namespace OpenRobo.Moderation;

public static class MuteExtensions
{
	public static void Mute(this SocketGuildUser user, long time)
	{
		SetUserMuted(user, MuteType.ChatMute, true, time);
	}
	public static void UnMute(this SocketGuildUser user)
	{
		SetUserMuted(user, MuteType.ChatMute, false);
	}

	public static void ReactMute(this SocketGuildUser user, long time)
	{
		SetUserMuted(user, MuteType.ReactMute, true, time);
	}
	public static void UnReactMute(this SocketGuildUser user)
	{
		SetUserMuted(user, MuteType.ReactMute, false);
	}

	public static void ImageMute(this SocketGuildUser user, long time)
	{
		SetUserMuted(user, MuteType.ReactMute, true, time);
	}
	public static void UnImageMute(this SocketGuildUser user)
	{
		SetUserMuted(user, MuteType.ReactMute, false);
	}
}
