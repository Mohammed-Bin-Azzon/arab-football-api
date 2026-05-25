using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArabFootball.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFanFavoriteCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FavoritePlayerCode",
                table: "Fans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FavoriteTeamCode",
                table: "Fans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoritePlayerCode",
                table: "Fans");

            migrationBuilder.DropColumn(
                name: "FavoriteTeamCode",
                table: "Fans");
        }
    }
}
