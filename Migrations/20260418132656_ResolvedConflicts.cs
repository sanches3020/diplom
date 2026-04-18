using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sofia.Web.Migrations
{
    /// <inheritdoc />
    public partial class ResolvedConflicts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PsychologistProfile_PsychologistProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Psychologists_AspNetUsers_UserId",
                table: "Psychologists");

            migrationBuilder.DropTable(
                name: "PsychologistProfile");

            migrationBuilder.DropIndex(
                name: "IX_Psychologists_UserId",
                table: "Psychologists");

            migrationBuilder.RenameColumn(
                name: "PsychologistProfileId",
                table: "AspNetUsers",
                newName: "PsychologistId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_PsychologistProfileId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_PsychologistId");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "forum",
                table: "Threads",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                schema: "forum",
                table: "Threads",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "PsychologistReviews",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PsychologistAppointments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "forum",
                table: "Posts",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "forum",
                table: "Posts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "AdminLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "AdminLogs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "AdminLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "AdminLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AdminLogs",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "AdminLogs",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Threads_AuthorId1",
                schema: "forum",
                table: "Threads",
                column: "AuthorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Psychologists_PsychologistId",
                table: "AspNetUsers",
                column: "PsychologistId",
                principalTable: "Psychologists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_AspNetUsers_AuthorId1",
                schema: "forum",
                table: "Threads",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Psychologists_PsychologistId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_AspNetUsers_AuthorId1",
                schema: "forum",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Threads_AuthorId1",
                schema: "forum",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                schema: "forum",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AdminLogs");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "AdminLogs");

            migrationBuilder.RenameColumn(
                name: "PsychologistId",
                table: "AspNetUsers",
                newName: "PsychologistProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_PsychologistId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_PsychologistProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "forum",
                table: "Threads",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "PsychologistReviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PsychologistAppointments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "forum",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "forum",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "AdminLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "AdminLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "AdminLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "AdminLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "PsychologistProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Education = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Experience = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Languages = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Methods = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Specialization = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychologistProfile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Psychologists_UserId",
                table: "Psychologists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PsychologistProfile_PsychologistProfileId",
                table: "AspNetUsers",
                column: "PsychologistProfileId",
                principalTable: "PsychologistProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Psychologists_AspNetUsers_UserId",
                table: "Psychologists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
