namespace OpenRobo.Database;

public class ServerConfig
{
	public ulong LevellingAnnouncementChannel { get; set; } = 0;
	public ulong DeletionLogChannel { get; set; } = 0;
	public ulong EditLogChannel { get; set; } = 0;
	public ulong DebugLogChannel { get; set; } = 0;
	public ulong MuteRole { get; set; } = 0;
	public List<ulong> SpamFilteredChannels { get; set; } = new();
	public Dictionary<int, ulong> LevellingRoleRewards { get; set; } = new();
}
