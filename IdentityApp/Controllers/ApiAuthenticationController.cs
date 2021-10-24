using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class ApiAuthenticationController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public ApiAuthenticationController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public class SignInCredentials
        {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }

        [HttpPost("signin")]
        public async Task<object> ApiSignIn([FromBody] SignInCredentials credentials)
        {
            var result = await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password,
                isPersistent: true, lockoutOnFailure: true);

            return new { success = result.Succeeded };
        }

        [HttpPost("signout")]
        public async Task<ActionResult> ApiSignOut()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
    }
}
