using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class ProgressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Progress",
                table: "CourseUser");

            migrationBuilder.AddColumn<bool>(
                name: "Listened",
                table: "CourseUser",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Progresses",
                columns: table => new
                {
                    ProgressId = table.Column<string>(nullable: false),
                    CourseUserCourseId = table.Column<string>(nullable: true),
                    CourseUserUserId = table.Column<string>(nullable: true),
                    ContentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progresses", x => x.ProgressId);
                    table.ForeignKey(
                        name: "FK_Progresses_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Progresses_CourseUser_CourseUserCourseId_CourseUserUserId",
                        columns: x => new { x.CourseUserCourseId, x.CourseUserUserId },
                        principalTable: "CourseUser",
                        principalColumns: new[] { "CourseId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_ContentId",
                table: "Progresses",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_CourseUserCourseId_CourseUserUserId",
                table: "Progresses",
                columns: new[] { "CourseUserCourseId", "CourseUserUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Progresses");

            migrationBuilder.DropColumn(
                name: "Listened",
                table: "CourseUser");

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "CourseUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
