using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackLog.Migrations
{
    /// <inheritdoc />
    public partial class AlbumYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Albums",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "Albums");
        }
    }
}
