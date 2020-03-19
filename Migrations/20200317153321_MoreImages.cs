using Microsoft.EntityFrameworkCore.Migrations;

namespace DualPrep.Migrations
{
    public partial class MoreImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Meal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Blog");
        }
    }
}
