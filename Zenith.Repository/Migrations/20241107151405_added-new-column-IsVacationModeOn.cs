using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addednewcolumnIsVacationModeOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVocationModeOn",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVocationModeOn",
                table: "AspNetUsers");
        }
    }
}
