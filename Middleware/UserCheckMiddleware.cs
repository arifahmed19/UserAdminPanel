using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using UserAdminPanel.Data;
using UserAdminPanel.Models;

namespace UserAdminPanel.Middleware
{
    public class UserCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public UserCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            if (path.StartsWith("/account/login") || 
                path.StartsWith("/account/register") || 
                path.StartsWith("/css/") || 
                path.StartsWith("/js/") || 
                path.StartsWith("/lib/"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                
                var user = db.Users.FirstOrDefault(u => u.Email == email);

                if (user == null || user.Status == UserStatus.Blocked)
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}
