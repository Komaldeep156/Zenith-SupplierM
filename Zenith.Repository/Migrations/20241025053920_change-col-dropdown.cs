using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class changecoldropdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropdownValues_DropdownLists_DropdownListsId",
                table: "DropdownValues");

            migrationBuilder.DropIndex(
                name: "IX_DropdownValues_DropdownListsId",
                table: "DropdownValues");

            migrationBuilder.DropColumn(
                name: "DropdownListsId",
                table: "DropdownValues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DropdownListsId",
                table: "DropdownValues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DropdownValues_DropdownListsId",
                table: "DropdownValues",
                column: "DropdownListsId");

            migrationBuilder.AddForeignKey(
                name: "FK_DropdownValues_DropdownLists_DropdownListsId",
                table: "DropdownValues",
                column: "DropdownListsId",
                principalTable: "DropdownLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
