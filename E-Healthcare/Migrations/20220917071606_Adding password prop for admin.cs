using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Healthcare.Migrations
{
    public partial class Addingpasswordpropforadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminPassword",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminPassword",
                table: "Users");
        }
    }
}
