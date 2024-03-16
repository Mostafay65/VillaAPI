using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class ImageForVilla2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Villas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://dotnetmastery.com/bluevillaimages/villa2.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "https://placehold.co/600*400");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "VillaNumbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://dotnetmastery.com/bluevillaimages/villa2.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "https://placehold.co/600*401");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Villas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://placehold.co/600*400",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "https://dotnetmastery.com/bluevillaimages/villa2.jpg");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "VillaNumbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://placehold.co/600*401",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "https://dotnetmastery.com/bluevillaimages/villa2.jpg");
        }
    }
}
