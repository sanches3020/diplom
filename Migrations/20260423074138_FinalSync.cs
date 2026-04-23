using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sofia.Web.Migrations
{
    /// <inheritdoc />
    public partial class FinalSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_AspNetUsers_PsychologistId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_PsychologistId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "PsychologistId",
                table: "Goals");
        }
    }
}
