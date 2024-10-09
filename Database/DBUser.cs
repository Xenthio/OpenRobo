using System.ComponentModel.DataAnnotations.Schema;

namespace OpenRobo.Database
{
	internal class User
	{
		[NotMapped]
		public string Username { get; set; } = "UnknownUser";
		public ulong ID { get; set; }
		public int Level { get; set; }
		public int XP { get; set; }

		[NotMapped]
		public bool IsMuted { get; set; }

		[NotMapped]
		public long MutedUntil { get; set; }

		[NotMapped]
		public long TimeLastXPReward { get; set; }
	}
}
