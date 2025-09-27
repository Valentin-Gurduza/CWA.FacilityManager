// Merged Program.cs (User-profile-history + Merge-Test branches)
// Includes: User profile services, Calendar task services, User/Role/Permission management, seeding, custom auth policies.

using System.Text.Json;
using System.Text.Json.Serialization;
using CWA.FacilityManager.Application.Interfaces;                   // General application interfaces (Calendar, Rooms, etc.)
using CWA.FacilityManager.Application.Interfaces.UserManagement;    // User management specific interfaces
using CWA.FacilityManager.Application.Services;                     // General application services
using CWA.FacilityManager.Application.Services.UserManagement;      // User management specific services
using CWA.FacilityManager.Client.Pages;
using CWA.FacilityManager.Client.Services;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Server.Components;
using CWA.FacilityManager.Server.Components.Account;
using CWA.FacilityManager.Server.Data;                              // SeedData
using CWA.FacilityManager.Server.Extensions;                        // Database seeding extensions
using CWA.FacilityManager.Server.Services;                          // ActiveUserRequirementHandler, RoleBasedRedirectService, EmailService, QrCodeService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// If you re-enable a custom DateTime converter, ensure the using below (was from CalendarManagement branch)
// using CWA.FacilityManager.Shared.Converters;

var builder = WebApplication.CreateBuilder(args);

// Razor + Interactive (Server + WASM) + Identity auth state serialization
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// Controllers + JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Re-add when ready:
        // options.JsonSerializerOptions.Converters.Add(new CalendarDateTimeConverter());
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCascadingAuthenticationState();

// Identity support helpers
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

// Authentication & Identity cookies
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailConfirmed", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("EmailConfirmed", "True"));

    options.AddPolicy("ActiveUser", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("IsActive", "True"));

    options.AddPolicy("ActiveAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("IsActive", "True")
              .RequireRole("Administrator"));

    options.AddPolicy("ActiveAdminOrSecretary", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("IsActive", "True")
              .RequireRole("Administrator", "Secretary"));
});

// Custom authorization handler
builder.Services.AddScoped<IAuthorizationHandler, ActiveUserRequirementHandler>();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("CWA.FacilityManager.Infrastructure")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity configuration (enhanced from Merge-Test branch with User-profile-history requirements)
builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        // Simplified for easier testing (from User-profile-history) but configurable for production
        options.SignIn.RequireConfirmedAccount =
            builder.Configuration.GetValue<bool>("Identity:RequireConfirmedAccount",
                builder.Environment.IsProduction() ? true : false); // False for development, true for production

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<EmailConfirmedClaimsPrincipalFactory>();

// AutoMapper (User Management mapping profile)
builder.Services.AddAutoMapper(typeof(CWA.FacilityManager.Application.Mappings.UserManagementMappingProfile));

// User Management Services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RoleInitializationService>();

// Room / Facility / Event Services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBuildingService, BuildingService>();
builder.Services.AddScoped<IEventService, EventService>();

// Calendar management
builder.Services.AddScoped<ICalendarTaskService, CalendarTaskService>();

// Email + QR Code + Role-based redirect
builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IRoleBasedRedirectService, RoleBasedRedirectService>();

// HttpContext + HttpClient (server-side base address)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    var baseAddress = request != null ? $"{request.Scheme}://{request.Host}" : "https://localhost:5001";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});

// Client calendar API service
builder.Services.AddScoped<ICalendarTaskApiService, CalendarTaskApiService>();

// Application services (from User-profile-history branch)
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// Add HttpClient for external calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Initialization / Seeding (combined approach from both branches)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // Core data seeding (from Merge-Test)
        var context = services.GetRequiredService<ApplicationDbContext>();
        await SeedData.Initialize(context);

        // System permissions & role-permission assignments (from Merge-Test)
        var permissionService = services.GetRequiredService<IPermissionService>();
        await permissionService.InitializeSystemPermissionsAsync();

        var roleInitService = services.GetRequiredService<RoleInitializationService>();
        await roleInitService.InitializeDefaultRolePermissionsAsync();

        logger.LogInformation("Application initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during application initialization.");
        
        // Fallback to User-profile-history approach if main seeding fails
        try
        {
            logger.LogInformation("Attempting fallback database seeding...");
            await app.SeedDatabaseAsync();
        }
        catch (Exception seedEx)
        {
            logger.LogWarning(seedEx, "Fallback seeding also failed. This might be normal during development if SQL Server is not running.");
        }
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticAssets();

// Authentication / Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CWA.FacilityManager.Client._Imports).Assembly);

// Identity endpoints
app.MapAdditionalIdentityEndpoints();

// API Controllers
app.MapControllers();

app.Run();