using Discord;
using Discord.WebSocket;
using System.Reflection;

namespace OpenRobo;

public class Events
{
	public static async Task LogMessage(LogMessage message)
	{
		Log.Message(message);
	}
	public static async Task UserJoined(SocketGuildUser user)
	{
		RunEvent<UserJoinedAttribute>(user);
	}
	public static async Task MessageUpdated(Cacheable<IMessage, ulong> cachedMessage, SocketMessage socketMessage, ISocketMessageChannel socketMessageChannel)
	{
		RunEvent<MessageUpdatedAttribute>(cachedMessage, socketMessage, socketMessageChannel);
	}
	public static async Task MessageRecieved(SocketMessage socketMessage)
	{
		RunEvent<MessageRecievedAttribute>(socketMessage);
	}
	public static async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
	{
		RunEvent<MessageDeletedAttribute>(cachedMessage, cachedChannel);
	}
	public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedUserMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction socketReaction)
	{
		RunEvent<ReactionAddedAttribute>(cachedUserMessage, cachedChannel, socketReaction);
	}

	public static void RunEvent<T>(params object[] prms)
	{
		var methods = Assembly.GetExecutingAssembly().GetTypes()
				  .SelectMany(t => t.GetMethods())
				  .Where(m => m.GetCustomAttributes(typeof(T), false).Length > 0)
				  .ToArray();
		foreach (var method in methods)
		{
			method.Invoke(null, prms);
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class UserJoinedAttribute : System.Attribute { }


	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class MessageUpdatedAttribute : System.Attribute { }


	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class MessageRecievedAttribute : System.Attribute { }


	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class MessageDeletedAttribute : System.Attribute { }


	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class ReactionAddedAttribute : System.Attribute { }
}
