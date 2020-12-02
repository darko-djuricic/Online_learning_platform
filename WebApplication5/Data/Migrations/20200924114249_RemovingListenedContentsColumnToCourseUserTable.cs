using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class RemovingListenedContentsColumnToCourseUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_CourseUser_CourseUserCourseId_CourseUserUserId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_CourseUserCourseId_CourseUserUserId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "CourseUserCourseId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "CourseUserUserId",
                table: "Contents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseUserCourseId",
                table: "Contents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseUserUserId",
                table: "Contents",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CourseUserCourseId_CourseUserUserId",
                table: "Contents",
                columns: new[] { "CourseUserCourseId", "CourseUserUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_CourseUser_CourseUserCourseId_CourseUserUserId",
                table: "Contents",
                columns: new[] { "CourseUserCourseId", "CourseUserUserId" },
                principalTable: "CourseUser",
                principalColumns: new[] { "CourseId", "UserId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
