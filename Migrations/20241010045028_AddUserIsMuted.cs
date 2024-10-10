using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenRobo.Migrations
{
	/// <inheritdoc />
	public partial class AddUserIsMuted : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsMuted",
				table: "Users",
				type: "INTEGER",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<long>(
				name: "MutedUntil",
				table: "Users",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0L);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsMuted",
				table: "Users");

			migrationBuilder.DropColumn(
				name: "MutedUntil",
				table: "Users");
		}
	}
}
