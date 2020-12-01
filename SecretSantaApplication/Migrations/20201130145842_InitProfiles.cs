using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApplication.Migrations
{
    public partial class InitProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    EmailAddress = table.Column<string>(nullable: false),
                    Age = table.Column<int>(nullable: false),
                    Wishes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.EmailAddress);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
