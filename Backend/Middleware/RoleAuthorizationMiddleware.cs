using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthcareApp.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            if (path.Contains("/api/"))
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                    return;
                }

                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                if (string.IsNullOrEmpty(userRole))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { error = "No role assigned" });
                    return;
                }

                if (context.Request.Method == "DELETE" && userRole != "Admin")
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { error = "Insufficient permissions" });
                    return;
                }

                if ((context.Request.Method == "POST" || context.Request.Method == "PUT") &&
                    userRole != "Admin" && userRole != "Manager")
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { error = "Insufficient permissions" });
                    return;
                }
            }

            await _next(context);
        }
    }
}
