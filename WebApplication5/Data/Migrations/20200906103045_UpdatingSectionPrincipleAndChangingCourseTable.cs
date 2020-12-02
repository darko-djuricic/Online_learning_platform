using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class UpdatingSectionPrincipleAndChangingCourseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Courses_CourseId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_CourseId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Contents");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Contents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    SectionId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    CourseId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.SectionId);
                    table.ForeignKey(
                        name: "FK_Section_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_SectionId",
                table: "Contents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Section_CourseId",
                table: "Section",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Section_SectionId",
                table: "Contents",
                column: "SectionId",
                principalTable: "Section",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Section_SectionId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Contents_SectionId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Contents");

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Contents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CourseId",
                table: "Contents",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Courses_CourseId",
                table: "Contents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
