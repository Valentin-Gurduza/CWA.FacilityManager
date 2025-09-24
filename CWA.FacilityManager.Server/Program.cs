using CWA.FacilityManager.Application.Interfaces.UserManagement;
using CWA.FacilityManager.Application.Services.UserManagement;
using CWA.FacilityManager.Client.Pages;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Server.Components;
using CWA.FacilityManager.Server.Components.Account;
using CWA.FacilityManager.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents() // Add this line for interactive server components
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Add authorization with enhanced policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailConfirmed", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("EmailConfirmed", "True"));
              
    // Add policy to require active users - CRITICAL SECURITY
    options.AddPolicy("ActiveUser", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("IsActive", "True"));
              
    // Combine policies for admin access
    options.AddPolicy("ActiveAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("IsActive", "True")
              .RequireRole("Administrator"));
              
    options.AddPolicy("ActiveAdminOrSecretary", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("IsActive", "True")
              .RequireRole("Administrator", "Secretary"));
});

// Register the custom authorization handler
builder.Services.AddScoped<IAuthorizationHandler, ActiveUserRequirementHandler>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with custom user and role
builder.Services.AddIdentityCore<ApplicationUser>(options => 
    {
        // Set email confirmation requirement from configuration
        // Configure in appsettings.json: "Identity": { "RequireConfirmedAccount": true }
        options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>("Identity:RequireConfirmedAccount", builder.Environment.IsProduction());
        
        // Configure password requirements
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        
        // Configure lockout
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        
        // Configure email confirmation
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<ApplicationRole>() // Add custom role support
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<EmailConfirmedClaimsPrincipalFactory>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(CWA.FacilityManager.Application.Mappings.UserManagementMappingProfile));

// Register User Management Services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RoleInitializationService>();

// Register email service and QR code service
builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();

// Register role-based redirect service
builder.Services.AddScoped<IRoleBasedRedirectService, RoleBasedRedirectService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode() // Add this line for interactive server render mode
    .AddAdditionalAssemblies(typeof(CWA.FacilityManager.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Initialize permissions and role assignments on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        // Initialize system permissions
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        await permissionService.InitializeSystemPermissionsAsync();
        
        // Initialize role permissions
        var roleInitService = scope.ServiceProvider.GetRequiredService<RoleInitializationService>();
        await roleInitService.InitializeDefaultRolePermissionsAsync();
        
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("User management system initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing user management system");
    }
}

app.Run();
