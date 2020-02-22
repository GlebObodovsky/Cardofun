using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Cardofun.API.Policies.Requirements
{
    public class AccessedUserMatchesCurrentRequirement : IAuthorizationRequirement {}
    public class AccessedUserMatchesCurrentHandler : AuthorizationHandler<AccessedUserMatchesCurrentRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccessedUserMatchesCurrentHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessedUserMatchesCurrentRequirement requirement)
        {
            _httpContextAccessor.HttpContext.Request.RouteValues
                .TryGetValue("userId", out object accessedUserId);

            var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(currentUserId) && currentUserId.Equals(accessedUserId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}