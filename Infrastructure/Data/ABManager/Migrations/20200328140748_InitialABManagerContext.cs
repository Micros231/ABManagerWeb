using Microsoft.EntityFrameworkCore.Migrations;

namespace ABManagerWeb.Infrastructure.Data.ABManager.Migrations
{
    public partial class InitialABManagerContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManifestInfos",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManifestInfos", x => new { x.Id, x.Path, x.Version });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManifestInfos");
        }
    }
}
