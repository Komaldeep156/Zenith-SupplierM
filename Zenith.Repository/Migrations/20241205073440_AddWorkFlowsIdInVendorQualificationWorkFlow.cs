using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkFlowsIdInVendorQualificationWorkFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkFlowsId",
                table: "VendorQualificationWorkFlow",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VendorQualificationWorkFlow_WorkFlowsId",
                table: "VendorQualificationWorkFlow",
                column: "WorkFlowsId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorQualificationWorkFlow_WorkFlows_WorkFlowsId",
                table: "VendorQualificationWorkFlow",
                column: "WorkFlowsId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorQualificationWorkFlow_WorkFlows_WorkFlowsId",
                table: "VendorQualificationWorkFlow");

            migrationBuilder.DropIndex(
                name: "IX_VendorQualificationWorkFlow_WorkFlowsId",
                table: "VendorQualificationWorkFlow");

            migrationBuilder.DropColumn(
                name: "WorkFlowsId",
                table: "VendorQualificationWorkFlow");
        }
    }
}
