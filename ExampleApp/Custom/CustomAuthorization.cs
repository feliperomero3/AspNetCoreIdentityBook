using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public class CustomAuthorization
    {
        private readonly RequestDelegate _next;

        public CustomAuthorization(RequestDelegate requestDelegate) => _next = requestDelegate;

        public async Task Invoke(HttpContext context)
        {
            if (context.GetEndpoint()?.DisplayName == "secret")
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    if (context.User.IsInRole("Administrator"))
                    {
                        await _next(context);
                    }
                    else
                    {
                        Forbid(context);
                    }
                }
                else
                {
                    Challenge(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        public void Challenge(HttpContext context) => context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        public void Forbid(HttpContext context) => context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
}
