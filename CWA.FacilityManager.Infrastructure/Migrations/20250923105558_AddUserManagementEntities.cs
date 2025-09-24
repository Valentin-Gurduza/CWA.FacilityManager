using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CWA.FacilityManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserManagementEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "AspNetUserRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                table: "AspNetUserRoles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "AspNetUserRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUserRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "AspNetUserRoles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetRoles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemRole",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "AspNetRoles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsSystemPermission = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "Description", "DisplayName", "IsSystemPermission", "Module", "Name", "Resource" },
                values: new object[,]
                {
                    { 1, "View", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6177), "Permission to view users", "View Users", true, "UserManagement", "UserManagement.Users.View", "Users" },
                    { 2, "Create", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6517), "Permission to create users", "Create Users", true, "UserManagement", "UserManagement.Users.Create", "Users" },
                    { 3, "Edit", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6522), "Permission to edit users", "Edit Users", true, "UserManagement", "UserManagement.Users.Edit", "Users" },
                    { 4, "Delete", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6524), "Permission to delete users", "Delete Users", true, "UserManagement", "UserManagement.Users.Delete", "Users" },
                    { 5, "Assign", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6559), "Permission to assign users", "Assign Users", true, "UserManagement", "UserManagement.Users.Assign", "Users" },
                    { 6, "Unassign", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6576), "Permission to unassign users", "Unassign Users", true, "UserManagement", "UserManagement.Users.Unassign", "Users" },
                    { 7, "View", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6580), "Permission to view roles", "View Roles", true, "UserManagement", "UserManagement.Roles.View", "Roles" },
                    { 8, "Create", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6582), "Permission to create roles", "Create Roles", true, "UserManagement", "UserManagement.Roles.Create", "Roles" },
                    { 9, "Edit", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6583), "Permission to edit roles", "Edit Roles", true, "UserManagement", "UserManagement.Roles.Edit", "Roles" },
                    { 10, "Delete", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6586), "Permission to delete roles", "Delete Roles", true, "UserManagement", "UserManagement.Roles.Delete", "Roles" },
                    { 11, "AssignPermissions", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6588), "Permission to assignpermissions roles", "AssignPermissions Roles", true, "UserManagement", "UserManagement.Roles.AssignPermissions", "Roles" },
                    { 12, "View", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6592), "Permission to view permissions", "View Permissions", true, "UserManagement", "UserManagement.Permissions.View", "Permissions" },
                    { 13, "Create", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6594), "Permission to create permissions", "Create Permissions", true, "UserManagement", "UserManagement.Permissions.Create", "Permissions" },
                    { 14, "Delete", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6596), "Permission to delete permissions", "Delete Permissions", true, "UserManagement", "UserManagement.Permissions.Delete", "Permissions" },
                    { 15, "View", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6599), "Permission to view facilities", "View Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.View", "Facilities" },
                    { 16, "Create", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6601), "Permission to create facilities", "Create Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.Create", "Facilities" },
                    { 17, "Edit", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6603), "Permission to edit facilities", "Edit Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.Edit", "Facilities" },
                    { 18, "Delete", new DateTime(2025, 9, 23, 10, 55, 56, 671, DateTimeKind.Utc).AddTicks(6605), "Permission to delete facilities", "Delete Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.Delete", "Facilities" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedAt",
                table: "AspNetUsers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Department",
                table: "AspNetUsers",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_AssignedAt",
                table: "AspNetUserRoles",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ExpiresAt",
                table: "AspNetUserRoles",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_IsActive",
                table: "AspNetUserRoles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_IsActive",
                table: "AspNetRoles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_IsSystemRole",
                table: "AspNetRoles",
                column: "IsSystemRole");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_Priority",
                table: "AspNetRoles",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IsSystemPermission",
                table: "Permissions",
                column: "IsSystemPermission");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module",
                table: "Permissions",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module_Resource_Action",
                table: "Permissions",
                columns: new[] { "Module", "Resource", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource",
                table: "Permissions",
                column: "Resource");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_GrantedAt",
                table: "RolePermissions",
                column: "GrantedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_IsActive",
                table: "RolePermissions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Department",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_AssignedAt",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ExpiresAt",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_IsActive",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_IsActive",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_IsSystemRole",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_Priority",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsSystemRole",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "AspNetRoles");
        }
    }
}
