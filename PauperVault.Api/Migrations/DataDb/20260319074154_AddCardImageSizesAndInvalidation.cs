using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PauperVault.Api.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class AddCardImageSizesAndInvalidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLargeUrl",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageNormalUrl",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "InvalidatedAt",
                table: "Cards",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLargeUrl",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ImageNormalUrl",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "InvalidatedAt",
                table: "Cards");
        }
    }
}
