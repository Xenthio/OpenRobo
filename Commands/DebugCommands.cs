using Discord.WebSocket;

namespace OpenRobo.Commands
{
	internal class DebugCommands
	{
		[ChatCommand("spam")]
		public static void Spam(SocketMessage socketMessage)
		{
			socketMessage.Channel.SendMessageAsync("LALALALALA");
		}
		[ChatCommand("realcsm")]
		public static void csm(SocketMessage socketMessage)
		{
			socketMessage.Channel.SendMessageAsync("https://tenor.com/view/csm-real-csm-realcsm-xenthio-glitches-gif-1640274726689165148");
		}
	}
}
