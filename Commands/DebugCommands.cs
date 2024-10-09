using Discord.WebSocket;
using static OpenRobo.CommandSystem;

namespace OpenRobo.Commands
{
	internal class DebugCommands
	{
		[ChatCommand("spam")]
		public static void Spam(SocketMessage socketMessage)
		{
			socketMessage.Channel.SendMessageAsync("LALALALALA");
		}
	}
}
