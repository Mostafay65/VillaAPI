using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class ImageForVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Villas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://placehold.co/600*400");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "VillaNumbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://placehold.co/600*401");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "VillaNumbers");
        }
    }
}
