using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class RenamingColumnRepliesId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepliesId",
                table: "Comments",
                newName: "CommentId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
