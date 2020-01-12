using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouse.Migrations
{
    public partial class SpecialTagsFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SpecialTags",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "SpecialTags",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
