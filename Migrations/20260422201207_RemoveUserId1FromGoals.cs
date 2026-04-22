using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sofia.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserId1FromGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_AspNetUsers_UserId1",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_UserId1",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "PsychologistId",
                table: "Goals",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_PsychologistId",
                table: "Goals",
                column: "PsychologistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_AspNetUsers_PsychologistId",
                table: "Goals",
                column: "PsychologistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ChatMessage",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ChatMessage",
                newName: "Timestamp");

            migrationBuilder.DropForeignKey(
                name: "FK_Goals_AspNetUsers_PsychologistId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_PsychologistId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "PsychologistId",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Goals",
                type: "text",
                nullable: false,
                defaultValue: "");

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
        }
    }
}
