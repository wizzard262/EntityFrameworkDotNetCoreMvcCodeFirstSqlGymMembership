using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymMembers.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GymMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GymMemberClassSession",
                columns: table => new
                {
                    GymMemberId = table.Column<int>(type: "int", nullable: false),
                    ClassSessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymMemberClassSession", x => new { x.GymMemberId, x.ClassSessionId });
                    table.ForeignKey(
                        name: "FK_GymMemberClassSession_ClassSession",
                        column: x => x.ClassSessionId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GymMemberClassSession_GymMember",
                        column: x => x.GymMemberId,
                        principalTable: "GymMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ClassSessions",
                columns: new[] { "Id", "Capacity", "Name", "StartTime" },
                values: new object[,]
                {
                    { 1, 20, "Morning Yoga", new DateTime(2024, 6, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 15, "HIIT Blast", new DateTime(2024, 6, 1, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 25, "Spin Class", new DateTime(2024, 6, 2, 7, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "GymMembers",
                columns: new[] { "Id", "FirstName", "JoinDate", "LastName" },
                values: new object[,]
                {
                    { 1, "Alice", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Johnson" },
                    { 2, "Bob", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smith" },
                    { 3, "Charlie", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brown" }
                });

            migrationBuilder.InsertData(
                table: "GymMemberClassSession",
                columns: new[] { "ClassSessionId", "GymMemberId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymMemberClassSession_ClassSessionId",
                table: "GymMemberClassSession",
                column: "ClassSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymMemberClassSession");

            migrationBuilder.DropTable(
                name: "ClassSessions");

            migrationBuilder.DropTable(
                name: "GymMembers");
        }
    }
}
