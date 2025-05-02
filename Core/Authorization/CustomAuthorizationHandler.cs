using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Authorization
{
    public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizationRequrement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CustomAuthorizationRequrement requirement)
        {
            var roleClaim = context.User.FindFirst(r => r.Type == ClaimTypes.Role);
            if(roleClaim is not null && requirement.AllowedRoles.Contains(roleClaim.Value))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

    }
}
