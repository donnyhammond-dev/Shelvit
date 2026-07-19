using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackLog.Migrations
{
    /// <inheritdoc />
    public partial class AddSortTitleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SortTitle",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SortAuthor",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SortTitle",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SortArtist",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SortTitle",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortTitle",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SortAuthor",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SortTitle",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SortArtist",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "SortTitle",
                table: "Albums");
        }
    }
}
