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
	}
}
