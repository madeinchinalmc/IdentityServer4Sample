using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridClientApplication.AuthRequirements
{
    public class CustomHandler : AuthorizationHandler<SmithInRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SmithInRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
