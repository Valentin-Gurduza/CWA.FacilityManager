using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CWA.FacilityManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Activity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "Rooms");
        }
    }
}
