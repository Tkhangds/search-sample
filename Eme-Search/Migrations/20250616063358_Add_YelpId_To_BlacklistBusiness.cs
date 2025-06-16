using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eme_Search.Migrations
{
    /// <inheritdoc />
    public partial class Add_YelpId_To_BlacklistBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YelpId",
                table: "BlacklistBusinesses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YelpId",
                table: "BlacklistBusinesses");
        }
    }
}
