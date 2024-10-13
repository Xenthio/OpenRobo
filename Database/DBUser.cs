using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenRobo.Database
{
	internal class User
	{
		[NotMapped]
		public string Username { get; set; } = "UnknownUser";
		[Required]
		public ulong ID { get; set; }
		public int Level { get; set; }
		public int XP { get; set; }

		public bool IsMuted { get; set; }
		public long MutedUntil { get; set; }

		public bool IsImageMuted { get; set; }
		public long ImageMutedUntil { get; set; }

		public bool IsReactMuted { get; set; }
		public long ReactMutedUntil { get; set; }
	}
}
