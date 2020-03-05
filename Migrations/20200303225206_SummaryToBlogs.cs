using Microsoft.EntityFrameworkCore.Migrations;

namespace DualPrep.Migrations
{
    public partial class SummaryToBlogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Blog");
        }
    }
}
