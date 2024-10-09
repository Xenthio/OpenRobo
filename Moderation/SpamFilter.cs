using Discord.WebSocket;
using OpenRobo.Database;

namespace OpenRobo.Moderation
{
	internal class SpamFilter
	{
		[Events.MessageRecieved]
		public static void MessageRecieved(SocketMessage socketMessage)
		{
			var db = ServerDatabase.LoadOrCreateServerDB((socketMessage.Channel as SocketGuildChannel).Guild);
			var userdata = db.GetOrRegisterUser(socketMessage.Author);
			int i = userdata.Level;
		}
	}
}
