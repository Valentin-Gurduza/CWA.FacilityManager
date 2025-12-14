// CWA Facility Manager - Optimized Program.cs
// Clean Architecture with proper service registration and middleware configuration

using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Application.Interfaces.UserManagement;
using CWA.FacilityManager.Application.Services;
using CWA.FacilityManager.Application.Services.UserManagement;
using CWA.FacilityManager.Client.Services;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Server.Components;
using CWA.FacilityManager.Server.Components.Account;
using CWA.FacilityManager.Server.Data;
using CWA.FacilityManager.Server.Extensions;
using CWA.FacilityManager.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using UserManagementRoleInitService = CWA.FacilityManager.Application.Services.UserManagement.RoleInitializationService;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURATION
// ============================================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ============================================================================
// BLAZOR & RAZOR COMPONENTS
// ============================================================================

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// ============================================================================
// API CONTROLLERS & JSON CONFIGURATION
// ============================================================================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ============================================================================
// RESPONSE COMPRESSION (Performance Optimization)
// ============================================================================

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/javascript",
        "application/json",
        "text/css",
        "text/html",
        "text/json",
        "text/plain"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// ============================================================================
// MEMORY CACHING (Performance Optimization)
// ============================================================================

builder.Services.AddMemoryCache();

// ============================================================================
// AUTHENTICATION & AUTHORIZATION
// ============================================================================

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Basic policies
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

    // Role-level based policies
    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.AdministratorOnly, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Administrator)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.SecretaryOrHigher, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.RenterOrHigher, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Renter)));

    // Feature-based policies
    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageUsers, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Administrator)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageRoles, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Administrator)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageEvents, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanApproveEvents, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageRooms, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageBuildings, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Administrator)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanViewAllReservations, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageReservations, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanCreateReservations, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Renter)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanViewReports, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Secretary)));

    options.AddPolicy(CWA.FacilityManager.Application.Authorization.Policies.CanManageSystem, policy =>
        policy.RequireAuthenticatedUser()
              .Requirements.Add(new CWA.FacilityManager.Application.Authorization.RoleLevelRequirement(
                  CWA.FacilityManager.Application.Authorization.RoleLevels.Administrator)));
});

// Authorization Handlers
builder.Services.AddScoped<IAuthorizationHandler, ActiveUserRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CWA.FacilityManager.Application.Authorization.RoleLevelHandler>();

// ============================================================================
// DATABASE & ENTITY FRAMEWORK
// ============================================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("CWA.FacilityManager.Infrastructure");
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(30);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ============================================================================
// IDENTITY CONFIGURATION
// ============================================================================

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Get settings from configuration
    var securityConfig = builder.Configuration.GetSection("Security");
    
    options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>(
        "Identity:RequireConfirmedAccount",
        builder.Environment.IsProduction());

    // Password policy from configuration
    options.Password.RequiredLength = securityConfig.GetValue("PasswordPolicy:RequiredLength", 8);
    options.Password.RequireDigit = securityConfig.GetValue("PasswordPolicy:RequireDigit", true);
    options.Password.RequireLowercase = securityConfig.GetValue("PasswordPolicy:RequireLowercase", true);
    options.Password.RequireUppercase = securityConfig.GetValue("PasswordPolicy:RequireUppercase", true);
    options.Password.RequireNonAlphanumeric = securityConfig.GetValue("PasswordPolicy:RequireNonAlphanumeric", false);

    // Lockout settings from configuration
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
        securityConfig.GetValue("Lockout:DefaultLockoutTimeSpanMinutes", 15));
    options.Lockout.MaxFailedAccessAttempts = securityConfig.GetValue("Lockout:MaxFailedAccessAttempts", 5);
    options.Lockout.AllowedForNewUsers = securityConfig.GetValue("Lockout:AllowedForNewUsers", true);

    options.User.RequireUniqueEmail = true;
})
.AddRoles<ApplicationRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<EmailConfirmedClaimsPrincipalFactory>();

// ============================================================================
// AUTOMAPPER
// ============================================================================

builder.Services.AddAutoMapper(typeof(CWA.FacilityManager.Application.Mappings.UserManagementMappingProfile));

// ============================================================================
// APPLICATION SERVICES
// ============================================================================

// User Management Services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<UserManagementRoleInitService>();

// Facility Services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBuildingService, BuildingService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICalendarTaskService, CalendarTaskService>();

// Profile & Audit Services
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Infrastructure Services
builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IRoleBasedRedirectService, RoleBasedRedirectService>();

// Client Services
builder.Services.AddScoped<ICalendarTaskApiService, CalendarTaskApiService>();

// ============================================================================
// HTTP SERVICES
// ============================================================================

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<HttpClient>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    var baseAddress = request != null 
        ? $"{request.Scheme}://{request.Host}" 
        : "https://localhost:5001";
    
    return new HttpClient 
    { 
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddHttpClient();

// ============================================================================
// HEALTH CHECKS
// ============================================================================

builder.Services.AddHealthChecks();

// ============================================================================
// BUILD APPLICATION
// ============================================================================

var app = builder.Build();

// ============================================================================
// DATABASE INITIALIZATION & SEEDING
// ============================================================================

await InitializeDatabaseAsync(app);

// ============================================================================
// MIDDLEWARE PIPELINE
// ============================================================================

// Development-specific middleware
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

// Response compression (before static files for better performance)
app.UseResponseCompression();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// ============================================================================
// ENDPOINT MAPPING
// ============================================================================

// Health check endpoint
app.MapHealthChecks("/health");

// Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CWA.FacilityManager.Client._Imports).Assembly);

// Static assets for WebAssembly
app.MapStaticAssets();

// Identity endpoints
app.MapAdditionalIdentityEndpoints();

// API Controllers
app.MapControllers();

// ============================================================================
// RUN APPLICATION
// ============================================================================

app.Run();

// ============================================================================
// HELPER METHODS
// ============================================================================

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database initialization...");
        
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database exists and apply migrations
        if (app.Environment.IsDevelopment())
        {
            await context.Database.EnsureCreatedAsync();
        }
        
        // Seed initial data
        await SeedData.Initialize(context);

        // Seed default administrator
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await SeedData.SeedDefaultAdminUser(userManager, roleManager, logger);

        // Initialize permissions
        var permissionService = services.GetRequiredService<IPermissionService>();
        await permissionService.InitializeSystemPermissionsAsync();

        // Initialize role permissions
        var roleInitService = services.GetRequiredService<UserManagementRoleInitService>();
        await roleInitService.InitializeDefaultRolePermissionsAsync();

        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization.");
        
        if (app.Environment.IsDevelopment())
        {
            logger.LogWarning("Attempting fallback database seeding...");
            try
            {
                await app.SeedDatabaseAsync();
            }
            catch (Exception seedEx)
            {
                logger.LogWarning(seedEx, "Fallback seeding failed. This is expected if the database is not available.");
            }
        }
    }
}