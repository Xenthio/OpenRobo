using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace OpenRobo.Database;

internal class ServerDatabase : DbContext
{
	public DbSet<User> Users { get; set; }

	public string DatabasePath { get; }

	public ServerDatabase(string databasePath)
	{
		DatabasePath = databasePath;
	}

	public ServerDatabase()
	{
		DatabasePath = "example";
	}

	// The following configures EF to create a Sqlite database file in the
	// special "local" folder for your platform.
	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DatabasePath}");
}
internal class ServerInstance
{
	public SocketGuild Server;
	public ServerDatabase Database;
	public static List<ServerInstance> Instances = new();
	public static ServerInstance GetOrCreateServerInstance(SocketGuild server)
	{
		if (Instances.Where(x => x.Server.Id == server.Id).FirstOrDefault() is ServerInstance SDB)
		{
			return SDB;
		}
		else
		{
			var db = new ServerInstance(server);
			Instances.Add(db);
			return db;
		}
	}
	public static void SaveAll()
	{
		foreach (var db in Instances)
		{
			db.Database.SaveChanges();
		}
	}
	public ServerInstance(SocketGuild server)
	{
		Server = server;
		var folder = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenRobo", "ServerData", $"{Server.Id}");
		var path = System.IO.Path.Join(folder, $"database.db");
		LoadOrCreateServerConfig();
		Database = new ServerDatabase(path);
		Database.Database.Migrate();
		//Database.Database.EnsureCreated();
		//if (Database.Database.GetPendingMigrations().Any())
		//Database.Database.Migrate();
		Log.Info($"Loading Database: {path}");
	}
	public User GetOrRegisterUser(SocketUser socketUser)
	{
		if (Database.Users.Where(x => x.ID == socketUser.Id).FirstOrDefault() is User user)
		{
			user.Username = socketUser.Username;
			return user;
		}
		else
		{
			return RegisterUser(socketUser);
		}
	}
	public User RegisterUser(SocketUser socketUser)
	{
		Log.Info($"Registering new User {socketUser.Username} in server ${Server.Id}");
		var newuser = new User()
		{
			Username = socketUser.Username,
			ID = socketUser.Id,
			Level = 0,
			XP = 0,
		};
		Database.Users.Add(newuser);
		Database.SaveChanges();
		return newuser;
	}

	public void SaveChanges() { Database.SaveChanges(); }


	// Config stuff

	public ServerConfig Config;
	public void LoadOrCreateServerConfig()
	{
		var folder = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenRobo", "ServerData", $"{Server.Id}");
		var path = System.IO.Path.Join(folder, "server-config.json");
		if (File.Exists(path))
		{
			var b = File.ReadAllText(path);
			Config = JsonSerializer.Deserialize<ServerConfig>(b);
		}
		else
		{
			Config = new ServerConfig();
			SaveServerConfig();
		}
	}
	public void SaveServerConfig()
	{
		var folder = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenRobo", "ServerData", $"{Server.Id}");
		var path = System.IO.Path.Join(folder, $"server-config.json");
		var serialised = JsonSerializer.Serialize(Config);
		if (!Directory.Exists(folder))
		{
			Directory.CreateDirectory(folder);
		}
		Log.Info($"Saving Server Config: {path}");
		File.WriteAllText(path, serialised);
	}
}
