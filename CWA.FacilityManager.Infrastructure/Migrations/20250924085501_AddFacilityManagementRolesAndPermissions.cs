using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CWA.FacilityManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityManagementRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsSystemRole", "LastModifiedAt", "ModifiedBy", "Name", "NormalizedName", "Priority", "RoleType" },
                values: new object[,]
                {
                    { "76d9321f-5a6f-4062-aebc-77dcd9555b7c", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "System administrator with full access to all features", true, true, null, null, "Administrator", "ADMINISTRATOR", 100, 2 },
                    { "83dd095f-6534-413a-b6aa-d64d6840deac", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secretary with access to manage bookings, view reports, and manage facilities", true, true, null, null, "Secretary", "SECRETARY", 80, 3 },
                    { "c08376e7-c0b2-4ad4-9fe2-69209dcaa16d", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Renter with access to view facilities and create booking requests", true, true, null, null, "Renter", "RENTER", 60, 4 }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "Description", "DisplayName", "IsSystemPermission", "Module", "Name", "Resource" },
                values: new object[,]
                {
                    { 19, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view rooms", "View Rooms", true, "FacilityManagement", "FacilityManagement.Rooms.View", "Rooms" },
                    { 20, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create rooms", "Create Rooms", true, "FacilityManagement", "FacilityManagement.Rooms.Create", "Rooms" },
                    { 21, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit rooms", "Edit Rooms", true, "FacilityManagement", "FacilityManagement.Rooms.Edit", "Rooms" },
                    { 22, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete rooms", "Delete Rooms", true, "FacilityManagement", "FacilityManagement.Rooms.Delete", "Rooms" },
                    { 23, "ViewSchedule", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to viewschedule rooms", "ViewSchedule Rooms", true, "FacilityManagement", "FacilityManagement.Rooms.ViewSchedule", "Rooms" },
                    { 24, "ManageSchedule", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to manageschedule rooms", "ManageSchedule Rooms", true, "FacilityManagement", "FacilityManagement.Rooms.ManageSchedule", "Rooms" },
                    { 25, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view events", "View Events", true, "EventManagement", "EventManagement.Events.View", "Events" },
                    { 26, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create events", "Create Events", true, "EventManagement", "EventManagement.Events.Create", "Events" },
                    { 27, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit events", "Edit Events", true, "EventManagement", "EventManagement.Events.Edit", "Events" },
                    { 28, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete events", "Delete Events", true, "EventManagement", "EventManagement.Events.Delete", "Events" },
                    { 29, "Approve", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to approve events", "Approve Events", true, "EventManagement", "EventManagement.Events.Approve", "Events" },
                    { 30, "Reject", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to reject events", "Reject Events", true, "EventManagement", "EventManagement.Events.Reject", "Events" },
                    { 31, "Cancel", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to cancel events", "Cancel Events", true, "EventManagement", "EventManagement.Events.Cancel", "Events" },
                    { 32, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view bookings", "View Bookings", true, "BookingManagement", "BookingManagement.Bookings.View", "Bookings" },
                    { 33, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create bookings", "Create Bookings", true, "BookingManagement", "BookingManagement.Bookings.Create", "Bookings" },
                    { 34, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit bookings", "Edit Bookings", true, "BookingManagement", "BookingManagement.Bookings.Edit", "Bookings" },
                    { 35, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete bookings", "Delete Bookings", true, "BookingManagement", "BookingManagement.Bookings.Delete", "Bookings" },
                    { 36, "ViewAll", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to viewall bookings", "ViewAll Bookings", true, "BookingManagement", "BookingManagement.Bookings.ViewAll", "Bookings" },
                    { 37, "Approve", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to approve bookings", "Approve Bookings", true, "BookingManagement", "BookingManagement.Bookings.Approve", "Bookings" },
                    { 38, "Reject", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to reject bookings", "Reject Bookings", true, "BookingManagement", "BookingManagement.Bookings.Reject", "Bookings" },
                    { 39, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view reports", "View Reports", true, "ReportManagement", "ReportManagement.Reports.View", "Reports" },
                    { 40, "Generate", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to generate reports", "Generate Reports", true, "ReportManagement", "ReportManagement.Reports.Generate", "Reports" },
                    { 41, "Export", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to export reports", "Export Reports", true, "ReportManagement", "ReportManagement.Reports.Export", "Reports" },
                    { 42, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view system configuration", "View Configuration", true, "SystemManagement", "SystemManagement.Configuration.View", "Configuration" },
                    { 43, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit system configuration", "Edit Configuration", true, "SystemManagement", "SystemManagement.Configuration.Edit", "Configuration" },
                    { 44, "Backup", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to backup system configuration", "Backup Configuration", true, "SystemManagement", "SystemManagement.Configuration.Backup", "Configuration" },
                    { 45, "Restore", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to restore system configuration", "Restore Configuration", true, "SystemManagement", "SystemManagement.Configuration.Restore", "Configuration" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_RoleType",
                table: "AspNetRoles",
                column: "RoleType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_RoleType",
                table: "AspNetRoles");

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

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "AspNetRoles");
        }
    }
}
