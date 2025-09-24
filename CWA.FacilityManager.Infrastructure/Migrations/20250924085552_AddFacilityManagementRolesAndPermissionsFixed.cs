using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CWA.FacilityManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityManagementRolesAndPermissionsFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "76d9321f-5a6f-4062-aebc-77dcd9555b7c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83dd095f-6534-413a-b6aa-d64d6840deac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c08376e7-c0b2-4ad4-9fe2-69209dcaa16d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsSystemRole", "LastModifiedAt", "ModifiedBy", "Name", "NormalizedName", "Priority", "RoleType" },
                values: new object[,]
                {
                    { "550e8400-e29b-41d4-a716-446655440001", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "System administrator with full access to all features", true, true, null, null, "Administrator", "ADMINISTRATOR", 100, 2 },
                    { "550e8400-e29b-41d4-a716-446655440002", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secretary with access to manage bookings, view reports, and manage facilities", true, true, null, null, "Secretary", "SECRETARY", 80, 3 },
                    { "550e8400-e29b-41d4-a716-446655440003", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Renter with access to view facilities and create booking requests", true, true, null, null, "Renter", "RENTER", 60, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "550e8400-e29b-41d4-a716-446655440001");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "550e8400-e29b-41d4-a716-446655440002");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "550e8400-e29b-41d4-a716-446655440003");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsSystemRole", "LastModifiedAt", "ModifiedBy", "Name", "NormalizedName", "Priority", "RoleType" },
                values: new object[,]
                {
                    { "76d9321f-5a6f-4062-aebc-77dcd9555b7c", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "System administrator with full access to all features", true, true, null, null, "Administrator", "ADMINISTRATOR", 100, 2 },
                    { "83dd095f-6534-413a-b6aa-d64d6840deac", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secretary with access to manage bookings, view reports, and manage facilities", true, true, null, null, "Secretary", "SECRETARY", 80, 3 },
                    { "c08376e7-c0b2-4ad4-9fe2-69209dcaa16d", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Renter with access to view facilities and create booking requests", true, true, null, null, "Renter", "RENTER", 60, 4 }
                });
        }
    }
}
