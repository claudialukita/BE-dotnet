using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class add_designers_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dresses_DesignerModel_DesignerId",
                table: "Dresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DesignerModel",
                table: "DesignerModel");

            migrationBuilder.RenameTable(
                name: "DesignerModel",
                newName: "Designers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Designers",
                table: "Designers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dresses_Designers_DesignerId",
                table: "Dresses",
                column: "DesignerId",
                principalTable: "Designers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dresses_Designers_DesignerId",
                table: "Dresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Designers",
                table: "Designers");

            migrationBuilder.RenameTable(
                name: "Designers",
                newName: "DesignerModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DesignerModel",
                table: "DesignerModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dresses_DesignerModel_DesignerId",
                table: "Dresses",
                column: "DesignerId",
                principalTable: "DesignerModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
