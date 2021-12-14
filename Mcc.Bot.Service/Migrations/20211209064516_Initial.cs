using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mcc.Bot.Service.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthenticationTokens",
                columns: table => new
                {
                    Secret = table.Column<string>(type: "text", nullable: false),
                    CanManagePermissions = table.Column<bool>(type: "boolean", nullable: false),
                    CanManageVacancies = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationTokens", x => x.Secret);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                });

            new DataSeeder().SeedTo(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationTokens");

            migrationBuilder.DropTable(
                name: "Vacancies");
        }
    }
}
