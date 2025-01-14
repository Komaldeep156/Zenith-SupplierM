using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class added_new_tbl_SecurityGroupFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowToDelete",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "AllowToEdit",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "AllowToView",
                table: "Fields");

            migrationBuilder.RenameColumn(
                name: "SectionName",
                table: "Fields",
                newName: "TabName");

            migrationBuilder.RenameColumn(
                name: "SecurityGroupCode",
                table: "Fields",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "FieldCode",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SecurityGroupFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityGroupFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityGroupFields_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityGroupFields_SecurityGroups_SecurityGroupId",
                        column: x => x.SecurityGroupId,
                        principalTable: "SecurityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityGroupFields_FieldId",
                table: "SecurityGroupFields",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityGroupFields_SecurityGroupId",
                table: "SecurityGroupFields",
                column: "SecurityGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityGroupFields");

            migrationBuilder.DropColumn(
                name: "FieldCode",
                table: "Fields");

            migrationBuilder.RenameColumn(
                name: "TabName",
                table: "Fields",
                newName: "SectionName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Fields",
                newName: "SecurityGroupCode");

            migrationBuilder.AddColumn<bool>(
                name: "AllowToDelete",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowToEdit",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowToView",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
