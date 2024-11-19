using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addednewcolumnApprover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproverId",
                table: "VacationRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VacationRequests_ApproverId",
                table: "VacationRequests",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_AspNetUsers_ApproverId",
                table: "VacationRequests",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_AspNetUsers_ApproverId",
                table: "VacationRequests");

            migrationBuilder.DropIndex(
                name: "IX_VacationRequests_ApproverId",
                table: "VacationRequests");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "VacationRequests");
        }
    }
}
