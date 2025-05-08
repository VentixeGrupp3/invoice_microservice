using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoiceMicroservice.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";

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

            if (string.Equals(suppliedKey, adminKey, StringComparison.Ordinal))
            {
                ctx.HttpContext.Items["Role"] = "Admin";
                // optionally add admin ID
            }
            else if (string.Equals(suppliedKey, userKey, StringComparison.Ordinal))
            {
                ctx.HttpContext.Items["Role"] = "User";

                // If you're using a second header to identify user
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

            await next();
        }
    }
}
