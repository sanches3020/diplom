using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sofia.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Room = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AvatarCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MessageId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    PostId = table.Column<int>(type: "integer", nullable: true),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedByAdminId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminComment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarned = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complaints_AspNetUsers_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Complaints_AspNetUsers_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Complaints_ChatMessage_MessageId",
                        column: x => x.MessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Complaints_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "forum",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CreatedAt",
                table: "Complaints",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_MessageId",
                table: "Complaints",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_PostId",
                table: "Complaints",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ReviewedByAdminId",
                table: "Complaints",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_SenderUserId",
                table: "Complaints",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Status",
                table: "Complaints",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TargetUserId",
                table: "Complaints",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TargetUserId_Status",
                table: "Complaints",
                columns: new[] { "TargetUserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "ChatMessage");
        }
    }
}
