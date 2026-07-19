using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackLog.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Albums",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Albums");
        }
    }
}
