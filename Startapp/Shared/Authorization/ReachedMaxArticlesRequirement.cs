using Microsoft.AspNetCore.Authorization;
using Startapp.Shared.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Startapp.Shared.Authorization
{
    public class ReachedMaxArticlesRequirement : IAuthorizationRequirement
    {
        public int MaxArticle { get; set; }
        public ReachedMaxArticlesRequirement(int maxArticle)
        {
            MaxArticle = maxArticle;
        }
    }

    public class ReachedMaxArticlesHandler : AuthorizationHandler<ReachedMaxArticlesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReachedMaxArticlesRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                return Task.CompletedTask;
            }

            var user = context.User;
            var claim = context.User.FindFirst("MaxArticle");
            if (claim != null)
            {
                var max = int.Parse(claim?.Value);
                if (max <= requirement.MaxArticle)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
