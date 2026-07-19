using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackLog.Migrations
{
    /// <inheritdoc />
    public partial class AddShareTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlbumShareToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookShareToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameShareToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumShareToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BookShareToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GameShareToken",
                table: "AspNetUsers");
        }
    }
}
