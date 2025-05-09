using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoiceMicroservice.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string? _requiredRole;
        private const string ApiKeyHeaderName = "x-api-key";

        public ApiKeyAuthorizeAttribute(string? requiredRole = null)
        {
            _requiredRole = requiredRole;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var config = ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var adminKey = config.GetValue<string>("ApiKeys:Admin");
            var userKey = config.GetValue<string>("ApiKeys:User");

            if (!ctx.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var suppliedKey))
            {
                ctx.Result = new UnauthorizedResult();
                return;
            }

            string? role = null;

            if (string.Equals(suppliedKey, adminKey, StringComparison.Ordinal))
            {
                role = "Admin";
            }
            else if (string.Equals(suppliedKey, userKey, StringComparison.Ordinal))
            {
                role = "User";

                // Require x-user-id for user role
                if (!ctx.HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId) || string.IsNullOrWhiteSpace(userId))
                {
                    ctx.Result = new UnauthorizedObjectResult("Missing x-user-id header");
                    return;
                }

                ctx.HttpContext.Items["UserId"] = userId.ToString();
            }
            else
            {
                ctx.Result = new ForbidResult();
                return;
            }

            ctx.HttpContext.Items["Role"] = role;

            // Enforce required role if specified
            if (!string.IsNullOrEmpty(_requiredRole) && !string.Equals(role, _requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                ctx.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}

