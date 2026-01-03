using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthcareApp.Data;
using HealthcareApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HealthcareApp.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            if (context.Request.Path.Value.Contains("/api/") &&
                (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE"))
            {
                using var scope = context.RequestServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "anonymous";
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                var audit = new SearchAudit
                {
                    UserId = userId,
                    EntityType = "API",
                    SearchTerm = $"{context.Request.Method} {context.Request.Path}",
                    ResultCount = context.Response.StatusCode,
                    SearchedAt = DateTime.UtcNow,
                    IpAddress = ipAddress
                };

                dbContext.SearchAudits.Add(audit);
                await dbContext.SaveChangesAsync();
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
