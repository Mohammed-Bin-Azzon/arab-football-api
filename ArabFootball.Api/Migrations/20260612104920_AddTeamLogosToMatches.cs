using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArabFootball.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamLogosToMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwayTeamLogoUrl",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeTeamLogoUrl",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayTeamLogoUrl",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "HomeTeamLogoUrl",
                table: "Matches");
        }
    }
}
