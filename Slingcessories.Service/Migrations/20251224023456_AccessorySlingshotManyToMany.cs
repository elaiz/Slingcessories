using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slingcessories.Service.Migrations
{
    /// <inheritdoc />
    public partial class AccessorySlingshotManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accessories_Slingshots_SlingId",
                table: "Accessories");

            migrationBuilder.DropIndex(
                name: "IX_Accessories_SlingId",
                table: "Accessories");

            migrationBuilder.DropColumn(
                name: "SlingId",
                table: "Accessories");

            migrationBuilder.CreateTable(
                name: "AccessorySlingshots",
                columns: table => new
                {
                    AccessoryId = table.Column<int>(type: "int", nullable: false),
                    SlinghotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessorySlingshots", x => new { x.AccessoryId, x.SlinghotId });
                    table.ForeignKey(
                        name: "FK_AccessorySlingshots_Accessories_AccessoryId",
                        column: x => x.AccessoryId,
                        principalTable: "Accessories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessorySlingshots_Slingshots_SlinghotId",
                        column: x => x.SlinghotId,
                        principalTable: "Slingshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessorySlingshots_SlinghotId",
                table: "AccessorySlingshots",
                column: "SlinghotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessorySlingshots");

            migrationBuilder.AddColumn<int>(
                name: "SlingId",
                table: "Accessories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accessories_SlingId",
                table: "Accessories",
                column: "SlingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accessories_Slingshots_SlingId",
                table: "Accessories",
                column: "SlingId",
                principalTable: "Slingshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
