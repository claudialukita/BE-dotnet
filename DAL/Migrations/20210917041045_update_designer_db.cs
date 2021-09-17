using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class update_designer_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DesignerId",
                table: "Dresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DesignerModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignerModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dresses_DesignerId",
                table: "Dresses",
                column: "DesignerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dresses_DesignerModel_DesignerId",
                table: "Dresses",
                column: "DesignerId",
                principalTable: "DesignerModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dresses_DesignerModel_DesignerId",
                table: "Dresses");

            migrationBuilder.DropTable(
                name: "DesignerModel");

            migrationBuilder.DropIndex(
                name: "IX_Dresses_DesignerId",
                table: "Dresses");

            migrationBuilder.DropColumn(
                name: "DesignerId",
                table: "Dresses");
        }
    }
}
