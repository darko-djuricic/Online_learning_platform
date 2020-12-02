using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class RemoveMyCoursesPropUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Users_TeacherId",
                table: "CourseUser");

            migrationBuilder.DropIndex(
                name: "IX_CourseUser_TeacherId",
                table: "CourseUser");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "CourseUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "CourseUser",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_TeacherId",
                table: "CourseUser",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Users_TeacherId",
                table: "CourseUser",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
