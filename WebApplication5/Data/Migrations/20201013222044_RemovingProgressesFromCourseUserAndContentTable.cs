using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class RemovingProgressesFromCourseUserAndContentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "LikedComments");

            //migrationBuilder.DropColumn(
            //    name: "LikedComments",
            //    table: "Comments");

            //migrationBuilder.CreateTable(
            //    name: "Likes",
            //    columns: table => new
            //    {
            //        UserId = table.Column<string>(nullable: false),
            //        IdComment = table.Column<string>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Likes", x => new { x.IdComment, x.UserId });
            //        table.ForeignKey(
            //            name: "FK_Likes_Comments_IdComment",
            //            column: x => x.IdComment,
            //            principalTable: "Comments",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Likes_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Likes_UserId",
            //    table: "Likes",
            //    column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.DropTable(
            //        name: "Likes");

            //    migrationBuilder.AddColumn<int>(
            //        name: "LikedComments",
            //        table: "Comments",
            //        type: "int",
            //        nullable: false,
            //        defaultValue: 0);

            //    migrationBuilder.CreateTable(
            //        name: "LikedComments",
            //        columns: table => new
            //        {
            //            IdComment = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            UserId = table.Column<string>(type: "nvarchar(50)", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_LikedComments", x => new { x.IdComment, x.UserId });
            //            table.ForeignKey(
            //                name: "FK_LikedComments_Comments_IdComment",
            //                column: x => x.IdComment,
            //                principalTable: "Comments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_LikedComments_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "UserId",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateIndex(
            //        name: "IX_LikedComments_UserId",
            //        table: "LikedComments",
            //        column: "UserId");
        }
    }
}
