using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sofia.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_AspNetUsers_UserId1",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_UserId1",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_AspNetUsers_UserId1",
                schema: "forum",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId1",
                schema: "forum",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_UserId1",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_AspNetUsers_AuthorId1",
                schema: "forum",
                table: "Threads");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AspNetUsers_UserId1",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_UserId1",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Threads_AuthorId1",
                schema: "forum",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_UserId1",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId1",
                schema: "forum",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_PostLikes_UserId1",
                schema: "forum",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId1",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Goals_UserId1",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                schema: "forum",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                schema: "forum",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "forum",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Goals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserAnswers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                schema: "forum",
                table: "Threads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "TestResults",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                schema: "forum",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                schema: "forum",
                table: "PostLikes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Notes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Goals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_UserId1",
                table: "UserAnswers",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_AuthorId1",
                schema: "forum",
                table: "Threads",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_UserId1",
                table: "TestResults",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId1",
                schema: "forum",
                table: "Posts",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_UserId1",
                schema: "forum",
                table: "PostLikes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId1",
                table: "Notes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId1",
                table: "Goals",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_AspNetUsers_UserId1",
                table: "Goals",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_UserId1",
                table: "Notes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_AspNetUsers_UserId1",
                schema: "forum",
                table: "PostLikes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId1",
                schema: "forum",
                table: "Posts",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_AspNetUsers_UserId1",
                table: "TestResults",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_AspNetUsers_AuthorId1",
                schema: "forum",
                table: "Threads",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AspNetUsers_UserId1",
                table: "UserAnswers",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
