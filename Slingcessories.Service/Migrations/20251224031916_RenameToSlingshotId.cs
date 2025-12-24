using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slingcessories.Service.Migrations
{
    /// <inheritdoc />
    public partial class RenameToSlingshotId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessorySlingshots_Slingshots_SlinghotId",
                table: "AccessorySlingshots");

            migrationBuilder.RenameColumn(
                name: "SlinghotId",
                table: "AccessorySlingshots",
                newName: "SlingshotId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessorySlingshots_SlinghotId",
                table: "AccessorySlingshots",
                newName: "IX_AccessorySlingshots_SlingshotId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessorySlingshots_Slingshots_SlingshotId",
                table: "AccessorySlingshots",
                column: "SlingshotId",
                principalTable: "Slingshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessorySlingshots_Slingshots_SlingshotId",
                table: "AccessorySlingshots");

            migrationBuilder.RenameColumn(
                name: "SlingshotId",
                table: "AccessorySlingshots",
                newName: "SlinghotId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessorySlingshots_SlingshotId",
                table: "AccessorySlingshots",
                newName: "IX_AccessorySlingshots_SlinghotId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessorySlingshots_Slingshots_SlinghotId",
                table: "AccessorySlingshots",
                column: "SlinghotId",
                principalTable: "Slingshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
