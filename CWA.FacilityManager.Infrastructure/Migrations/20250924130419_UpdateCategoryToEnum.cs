using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CWA.FacilityManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "CalendarTasks",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "CalendarTasks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);
        }
    }
}
