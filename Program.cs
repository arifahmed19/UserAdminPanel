using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAdminPanel.Data;
using UserAdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);

// Connect to PostgreSQL (Supabase) - Force IPv4 to bypass Render networking issues
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

try
{
    var csBuilder = new NpgsqlConnectionStringBuilder(connectionString);
    if (!string.IsNullOrEmpty(csBuilder.Host))
    {
        var addresses = System.Net.Dns.GetHostAddresses(csBuilder.Host);
        var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        if (ipv4 != null)
        {
            Console.WriteLine($"[IPV4 FIX] Forcing {csBuilder.Host} to {ipv4}");
            csBuilder.Host = ipv4.ToString();
            connectionString = csBuilder.ToString();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[IPV4 FIX ERROR] DNS resolution failed: {ex.Message}");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration Error: {ex.Message}");
    }
}

// RESTORE PROFESSIONAL ERROR HANDLING FOR PRODUCTION
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserAdminPanel.Middleware.UserCheckMiddleware>();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
