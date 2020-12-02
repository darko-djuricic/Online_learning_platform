using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class RemoveLikedPropFromUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_LikedBy",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_LikedBy",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LikedBy",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LikedBy",
                table: "Comments",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LikedBy",
                table: "Comments",
                column: "LikedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_LikedBy",
                table: "Comments",
                column: "LikedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
