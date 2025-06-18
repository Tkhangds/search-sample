using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eme_Search.Migrations
{
    /// <inheritdoc />
    public partial class Refactor_blacklist_business_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlacklistBusinesses");

            migrationBuilder.RenameColumn(
                name: "YelpId",
                table: "BlacklistBusinesses",
                newName: "Provider");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "BlacklistBusinesses",
                newName: "YelpId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BlacklistBusinesses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
