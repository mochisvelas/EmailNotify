using Microsoft.EntityFrameworkCore.Migrations;

namespace EmailNotify.Migrations
{
    public partial class ModNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "Notification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
