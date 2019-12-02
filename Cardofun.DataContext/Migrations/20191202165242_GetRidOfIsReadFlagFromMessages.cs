using Microsoft.EntityFrameworkCore.Migrations;

namespace Cardofun.DataContext.Migrations
{
    public partial class GetRidOfIsReadFlagFromMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                nullable: false,
                defaultValue: false);
        }
    }
}
