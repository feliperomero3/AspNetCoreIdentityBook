using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp
{
    public static class SecretEndpoint
    {
        public static async Task Endpoint(HttpContext context)
        {
            await context.Response.WriteAsync("This is a secret message.");
        }
    }
}
