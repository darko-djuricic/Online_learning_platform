using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class AddingPublishAttr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Published",
                table: "Contents");

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Courses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Published",
                table: "Courses");

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Contents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
