using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public static class CustomSignInAndSignOut
    {
        private const string cookieKey = "authUser";

        public static async Task SignIn(HttpContext context)
        {
            string user = context.Request.Query["user"];

            if (user != null)
            {
                context.Response.Cookies.Append(cookieKey, user, new CookieOptions { Secure = true, HttpOnly = true });
                await context.Response.WriteAsync($"Authenticated user: {user}");
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        public static async Task SignOut(HttpContext context)
        {
            context.Response.Cookies.Delete(cookieKey);
            await context.Response.WriteAsync("Signed out");
        }
    }
}
