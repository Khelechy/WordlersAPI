using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordlersAPI.Migrations
{
    public partial class lastChangePassMigff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfRounds",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfRounds",
                table: "Games");
        }
    }
}
