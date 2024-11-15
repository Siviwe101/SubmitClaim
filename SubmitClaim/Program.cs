using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SubmitClaim.Data;
using SubmitClaim.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString).EnableSensitiveDataLogging());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>() // Enable role management
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed roles and users
await SeedRolesAndUsers(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Claims}/{action=Create}/{id?}");
app.MapRazorPages();

await app.RunAsync();

/// <summary>
/// Seeds roles and users.
/// </summary>
async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Seed roles
    string[] roles = { "Admin", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed an Admin user
    var adminEmail = "admin@claims.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var createUserResult = await userManager.CreateAsync(adminUser, "Qwertyuiop@0987654321"); 
        if (createUserResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Seed a Manager user
    var managerEmail = "manager@claims.com";
    var managerUser = await userManager.FindByEmailAsync(managerEmail);
    if (managerUser == null)
    {
        managerUser = new IdentityUser
        {
            UserName = managerEmail,
            Email = managerEmail,
            EmailConfirmed = true
        };
        var createUserResult = await userManager.CreateAsync(managerUser, "Qwertyuiop@0987654321"); 
        if (createUserResult.Succeeded)
        {
            await userManager.AddToRoleAsync(managerUser, "Manager");
        }
    }
}
