using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HealthcareApp.Authorization
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            // Check both "role" claim and ClaimTypes.Role
            var userRole = context.User.Claims
                .FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

            if (userRole != null && requirement.Roles.Any(r => r.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
