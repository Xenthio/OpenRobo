using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenRobo.Migrations
{
    /// <inheritdoc />
    public partial class AddReactAndImageMute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ImageMutedUntil",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsImageMuted",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReactMuted",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ReactMutedUntil",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageMutedUntil",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsImageMuted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsReactMuted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReactMutedUntil",
                table: "Users");
        }
    }
}
