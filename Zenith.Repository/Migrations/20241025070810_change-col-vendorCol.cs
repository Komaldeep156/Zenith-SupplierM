using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class changecolvendorCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vendors_SupplierCategoryId",
                table: "Vendors",
                column: "SupplierCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_SupplierScopeId",
                table: "Vendors",
                column: "SupplierScopeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_DropdownValues_SupplierCategoryId",
                table: "Vendors",
                column: "SupplierCategoryId",
                principalTable: "DropdownValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_DropdownValues_SupplierScopeId",
                table: "Vendors",
                column: "SupplierScopeId",
                principalTable: "DropdownValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_DropdownValues_SupplierCategoryId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_DropdownValues_SupplierScopeId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_SupplierCategoryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_SupplierScopeId",
                table: "Vendors");
        }
    }
}
