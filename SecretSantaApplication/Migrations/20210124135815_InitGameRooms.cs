using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApplication.Migrations
{
    public partial class InitGameRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStarted",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserToRooms",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToRooms", x => new { x.EmailAddress, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToRooms_Rooms_Name",
                        column: x => x.Name,
                        principalTable: "Rooms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToRooms_Users_EmailAddress",
                        column: x => x.EmailAddress,
                        principalTable: "Users",
                        principalColumn: "EmailAddress",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToRooms_Name",
                table: "UserToRooms",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToRooms");

            migrationBuilder.DropColumn(
                name: "IsStarted",
                table: "Rooms");
        }
    }
}
