using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CWA.FacilityManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendarTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FacilityId = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceType = table.Column<int>(type: "int", nullable: true),
                    RecurrenceInterval = table.Column<int>(type: "int", nullable: true),
                    RecurrenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Category = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarTasks_AspNetUsers_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Amenities = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Activity = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BuildingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttendeeCount = table.Column<int>(type: "int", nullable: true),
                    SpecialRequirements = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Organizer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrganizerCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExpectedAttendees = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsSystemRole", "LastModifiedAt", "ModifiedBy", "Name", "NormalizedName", "Priority", "RoleType" },
                values: new object[,]
                {
                    { "550e8400-e29b-41d4-a716-446655440001", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "System administrator with full access to all features", true, true, null, null, "Administrator", "ADMINISTRATOR", 100, 2 },
                    { "550e8400-e29b-41d4-a716-446655440002", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secretary with access to manage bookings, view reports, and manage facilities", true, true, null, null, "Secretary", "SECRETARY", 50, 3 },
                    { "550e8400-e29b-41d4-a716-446655440003", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Renter with access to view facilities and create booking requests", true, true, null, null, "Renter", "RENTER", 10, 4 }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "Description", "DisplayName", "IsSystemPermission", "Module", "Name", "Resource" },
                values: new object[,]
                {
                    { 1, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view users", "View Users", true, "UserManagement", "UserManagement.Users.View", "Users" },
                    { 2, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create users", "Create Users", true, "UserManagement", "UserManagement.Users.Create", "Users" },
                    { 3, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit users", "Edit Users", true, "UserManagement", "UserManagement.Users.Edit", "Users" },
                    { 4, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete users", "Delete Users", true, "UserManagement", "UserManagement.Users.Delete", "Users" },
                    { 5, "Assign", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to assign users", "Assign Users", true, "UserManagement", "UserManagement.Users.Assign", "Users" },
                    { 6, "Unassign", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to unassign users", "Unassign Users", true, "UserManagement", "UserManagement.Users.Unassign", "Users" },
                    { 7, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view roles", "View Roles", true, "UserManagement", "UserManagement.Roles.View", "Roles" },
                    { 8, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create roles", "Create Roles", true, "UserManagement", "UserManagement.Roles.Create", "Roles" },
                    { 9, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit roles", "Edit Roles", true, "UserManagement", "UserManagement.Roles.Edit", "Roles" },
                    { 10, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete roles", "Delete Roles", true, "UserManagement", "UserManagement.Roles.Delete", "Roles" },
                    { 11, "AssignPermissions", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to assignpermissions roles", "AssignPermissions Roles", true, "UserManagement", "UserManagement.Roles.AssignPermissions", "Roles" },
                    { 12, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view permissions", "View Permissions", true, "UserManagement", "UserManagement.Permissions.View", "Permissions" },
                    { 13, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create permissions", "Create Permissions", true, "UserManagement", "UserManagement.Permissions.Create", "Permissions" },
                    { 14, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete permissions", "Delete Permissions", true, "UserManagement", "UserManagement.Permissions.Delete", "Permissions" },
                    { 15, "View", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to view facilities", "View Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.View", "Facilities" },
                    { 16, "Create", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to create facilities", "Create Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.Create", "Facilities" },
                    { 17, "Edit", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to edit facilities", "Edit Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.Edit", "Facilities" },
                    { 18, "Delete", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission to delete facilities", "Delete Facilities", true, "FacilityManagement", "FacilityManagement.Facilities.Delete", "Facilities" },
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

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
                name: "IX_AspNetRoles_RoleType",
                table: "AspNetRoles",
                column: "RoleType");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

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
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

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
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionType",
                table: "AuditLogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entity_EntityId",
                table: "AuditLogs",
                columns: new[] { "Entity", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Room_DateRange",
                table: "Bookings",
                columns: new[] { "RoomId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTasks_AssignedUserId",
                table: "CalendarTasks",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTasks_Category",
                table: "CalendarTasks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTasks_EndDate",
                table: "CalendarTasks",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTasks_StartDate",
                table: "CalendarTasks",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarTasks_Status",
                table: "CalendarTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ApprovedById",
                table: "Events",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedById",
                table: "Events",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Events_RoomId_StartDateTime_EndDateTime",
                table: "Events",
                columns: new[] { "RoomId", "StartDateTime", "EndDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_Status",
                table: "Events",
                column: "Status");

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

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistory_User_Date",
                table: "UserHistories",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "CalendarTasks");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserHistories");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Buildings");
        }
    }
}
