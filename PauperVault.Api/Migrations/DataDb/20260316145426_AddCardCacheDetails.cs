using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PauperVault.Api.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class AddCardCacheDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Power",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetName",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Toughness",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Power",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "SetName",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Toughness",
                table: "Cards");
        }
    }
}
