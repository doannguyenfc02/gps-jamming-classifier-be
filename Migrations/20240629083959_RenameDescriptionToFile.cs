using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gps_jamming_classifier_be.Migrations
{
    /// <inheritdoc />
    public partial class RenameDescriptionToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "SignalDatas",
                newName: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "SignalDatas",
                newName: "Description");
        }
    }
}
