using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gps_jamming_classifier_be.Migrations
{
    public partial class ChangeDataBase64ToText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DataBase64",
                table: "Spectrograms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DataBase64",
                table: "Spectrograms",
                type: "ntext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
