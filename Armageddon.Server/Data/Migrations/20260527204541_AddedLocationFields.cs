using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Armageddon.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedLocationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Users",
                type: "double precision",
                nullable: false,
                defaultValue: 6.4934000000000003);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Users",
                type: "double precision",
                nullable: false,
                defaultValue: 3.7206999999999999);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Users",
                type: "numeric",
                nullable: false,
                defaultValue: 5m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Users");
        }
    }
}
