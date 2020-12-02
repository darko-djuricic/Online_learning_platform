using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class LikeRealization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikedComments",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    IdComment = table.Column<string>(nullable: false, maxLength: 50)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.IdComment, x.UserId });
                    table.ForeignKey(
                        name: "FK_Likes_Comments_IdComment",
                        column: x => x.IdComment,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "LikedComments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikedComments");
        }
    }
}
