using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace OpenRobo.Database
{
	internal class ServerDatabase : DbContext
	{
		public static List<ServerDatabase> Databases = new();
		public static ServerDatabase LoadOrCreateServerDB(SocketGuild server)
		{
			if (Databases.Where(x => x.Server.Id == server.Id).FirstOrDefault() is ServerDatabase SDB)
			{
				return SDB;
			}
			else
			{
				var db = new ServerDatabase(server);
				Databases.Add(db);
				return db;
			}
		}
		public static void SaveAll()
		{
			foreach (var db in Databases)
			{
				db.SaveChanges();
			}
		}
		public DbSet<User> Users { get; set; }
		public SocketGuild Server;
		public string DatabasePath { get; }
		public ServerDatabase(SocketGuild server)
		{
			Server = server;
			LoadOrCreateServerConfig();

			var folder = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenRobo", "ServerData", $"{Server.Id}");
			DatabasePath = System.IO.Path.Join(folder, $"database.db");
			Log.Info($"Loading Database: {DatabasePath}");

			Database.EnsureCreated();
		}
		public User GetOrRegisterUser(SocketUser socketUser)
		{
			if (Users.Where(x => x.ID == socketUser.Id).FirstOrDefault() is User user)
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
			Users.Add(newuser);
			SaveChanges();
			return newuser;
		}

		// The following configures EF to create a Sqlite database file in the
		// special "local" folder for your platform.
		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DatabasePath}");



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
}
