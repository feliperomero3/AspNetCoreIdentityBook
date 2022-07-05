using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Controllers
{
    // To simulate an API that uses access tokens for validation.
    [Route("api/[controller]")]
    [ApiController]
    public class DemoExternalApiController : ControllerBase
    {
        private readonly Dictionary<string, string> _data = new()
            {
                { "token1", "This is Alice's external data" },
                { "token2", "This is Dora's external data" },
            };

        // The controller defines a single action that provides a JSON object
        // based on the token included in the Authorization request header.
        // This is a simple example, but it provides enough functionality to
        // demonstrate the use of an access token.
        [HttpGet]
        public ActionResult GetData([FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(authorization))
            {
                var token = authorization?[7..];

                if (!string.IsNullOrEmpty(token) && _data.ContainsKey(token))
                {
                    return Ok(new { data = _data[token] });
                }
            }
            return NotFound();
        }
    }
}
