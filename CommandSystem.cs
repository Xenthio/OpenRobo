using Discord.WebSocket;
using System.Reflection;

namespace OpenRobo;

internal class CommandSystem
{
	private static readonly string[] Prefixes = new string[]
	{
		"/",
		"1 third of a triforce rotated 270 degrees ",
		"1 third of a triforce rotated 90 degrees ",
		"1 third triforce ",
		"1/3 sideways triforce ",
		"1/3 triforce ",
		"1/3 triforce rotated clockwise 90 degrees ",
		"4chan bullet point ",
		"4chan indent ",
		">",
		"one third of a triforce flipped along the horizontal axis and then rotated counter-clockwise 90 degrees ",
		"?",
		"^ rotated counter clockwise 270 degrees ",
		"^ rotated counter-clockwise 270 degrees ",
		"^ rotated counterclockwise 270 degrees ",
		"a blown over tent ",
		"a third of a triforce ",
		"comedy angle ",
		"comedy chevron ",
		"comedy dorito ",
		"funny angle ",
		"funny corner ",
		"funny dorito ",
		"go go gadget ",
		"hey siri ",
		"meme angle ",
		"meme angle ",
		"meme corner ",
		"meme triangle ",
		"meme arrow ",
		"mirrored < ",
		"mountain that got blown over ",
		"one third of a triforce flipped along the horizontal axis and then rotated 270 degrees ",
		"portrait ^ ",
		"portrait mode v ",
		"portrait mountain ",
		"portrait v ",
		"rotated < ",
		"rotated ^ ",
		"rotated v ",
		"sideways ^ ",
		"sideways mountain ",
		"sv_",
		"tent that got blown over ",
		"v rotated 270 degrees ",
		"v rotated 90 degrees ",
		"v rotated clockwise 270 degrees ",
	};
	[Events.MessageRecieved]
	public static void MessageRecieved(SocketMessage socketMessage)
	{
		if (socketMessage.Author.IsWebhook) return;
		if (socketMessage.Author.IsBot) return;
		foreach (var prefix in Prefixes)
		{
			if (socketMessage.Content.StartsWith(prefix))
			{
				var split = socketMessage.Content.Substring(prefix.Length).Split(" ");
				var cmd = split.FirstOrDefault();
				if (String.IsNullOrWhiteSpace(cmd))
				{
					break;
				}
				Log.Info($"Recieved Command: {cmd}");
				RunCommand(cmd, socketMessage);
				break;
			}
		}
	}
	public static void RunCommand(string command, SocketMessage message)
	{
		var methods = Assembly.GetExecutingAssembly().GetTypes()
				.SelectMany(t => t.GetMethods())
				.Where(m => m.GetCustomAttributes(typeof(ChatCommandAttribute), false)
						.Where(n => n is ChatCommandAttribute cmd && cmd.Command.ToLower() == command.ToLower()).Any())
				.ToArray();
		foreach (var method in methods)
		{
			Log.Info($"Running Event {command}, method: {method.Name}");
			method.Invoke(null, new object[] { message });
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class ChatCommandAttribute : System.Attribute
	{
		public string Command;
		public ChatCommandAttribute(string cmd)
		{
			this.Command = cmd;
		}
	}
}
