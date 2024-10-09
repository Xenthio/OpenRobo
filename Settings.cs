using System.Text.Json;

namespace OpenRobo;

public class Settings
{
	public static GlobalConfig Config;
	public static void LoadOrCreateGlobalConfig()
	{
		var folder = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenRobo");
		var path = System.IO.Path.Join(folder, "global-config.json");
		if (File.Exists(path))
		{
			var b = File.ReadAllText(path);
			Config = JsonSerializer.Deserialize<GlobalConfig>(b);
		}
		else
		{
			Config = new GlobalConfig();
			SaveGlobalConfig();
		}
	}
	public static void SaveGlobalConfig()
	{
		var folder = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenRobo");
		var path = System.IO.Path.Join(folder, $"global-config.json");
		var serialised = JsonSerializer.Serialize(Config);
		if (!Directory.Exists(folder))
		{
			Directory.CreateDirectory(folder);
		}
		Log.Info($"Saving Server Config: {path}");
		File.WriteAllText(path, serialised);
	}
}

public class GlobalConfig
{
	public string Token { get; set; } = "your_token_here";
}