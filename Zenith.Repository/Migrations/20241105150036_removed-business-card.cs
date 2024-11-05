using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class removedbusinesscard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessCard",
                table: "VendorsInitializationForm");

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierCountryId",
                table: "VendorsInitializationForm",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VendorsInitializationForm_SupplierCountryId",
                table: "VendorsInitializationForm",
                column: "SupplierCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorsInitializationForm_DropdownValues_SupplierCountryId",
                table: "VendorsInitializationForm",
                column: "SupplierCountryId",
                principalTable: "DropdownValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorsInitializationForm_DropdownValues_SupplierCountryId",
                table: "VendorsInitializationForm");

            migrationBuilder.DropIndex(
                name: "IX_VendorsInitializationForm_SupplierCountryId",
                table: "VendorsInitializationForm");

            migrationBuilder.DropColumn(
                name: "SupplierCountryId",
                table: "VendorsInitializationForm");

            migrationBuilder.AddColumn<string>(
                name: "BusinessCard",
                table: "VendorsInitializationForm",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
