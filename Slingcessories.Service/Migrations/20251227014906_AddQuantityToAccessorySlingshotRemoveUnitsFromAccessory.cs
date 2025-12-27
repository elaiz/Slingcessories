using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slingcessories.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToAccessorySlingshotRemoveUnitsFromAccessory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Units",
                table: "Accessories");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AccessorySlingshots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AccessorySlingshots");

            migrationBuilder.AddColumn<int>(
                name: "Units",
                table: "Accessories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
