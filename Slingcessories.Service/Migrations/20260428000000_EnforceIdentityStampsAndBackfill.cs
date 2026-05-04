using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slingcessories.Service.Migrations
{
    /// <inheritdoc />
    public partial class EnforceIdentityStampsAndBackfill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [AspNetUsers]
                SET [SecurityStamp] = COALESCE([SecurityStamp], CONVERT(nvarchar(36), NEWID())),
                    [ConcurrencyStamp] = COALESCE([ConcurrencyStamp], CONVERT(nvarchar(36), NEWID()))
                WHERE [SecurityStamp] IS NULL OR [ConcurrencyStamp] IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
