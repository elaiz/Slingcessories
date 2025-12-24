using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slingcessories.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddSlingshotsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SlingId",
                table: "Accessories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Slingshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slingshots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accessories_SlingId",
                table: "Accessories",
                column: "SlingId");

            migrationBuilder.CreateIndex(
                name: "IX_Slingshots_Year_Model_Color",
                table: "Slingshots",
                columns: new[] { "Year", "Model", "Color" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accessories_Slingshots_SlingId",
                table: "Accessories",
                column: "SlingId",
                principalTable: "Slingshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accessories_Slingshots_SlingId",
                table: "Accessories");

            migrationBuilder.DropTable(
                name: "Slingshots");

            migrationBuilder.DropIndex(
                name: "IX_Accessories_SlingId",
                table: "Accessories");

            migrationBuilder.DropColumn(
                name: "SlingId",
                table: "Accessories");
        }
    }
}
