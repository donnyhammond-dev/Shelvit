using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackLog.Migrations
{
    /// <inheritdoc />
    public partial class AddGameImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomImageUrl",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomImageUrl",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Games");
        }
    }
}
