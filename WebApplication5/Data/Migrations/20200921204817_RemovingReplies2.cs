using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class RemovingReplies2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_RepliesId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_RepliesId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "RepliesId",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepliesId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RepliesId",
                table: "Comments",
                column: "RepliesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_RepliesId",
                table: "Comments",
                column: "RepliesId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
