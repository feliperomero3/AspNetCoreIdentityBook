using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityApp.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class ApiAuthenticationController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public ApiAuthenticationController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration Configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = Configuration;
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
            var user = await _userManager.FindByEmailAsync(credentials.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, credentials.Password, true);

            if (result.Succeeded)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = (await _signInManager.CreateUserPrincipalAsync(user)).Identities.First(),
                    NotBefore = DateTime.Now.AddMinutes(-int.Parse(_configuration["BearerTokens:ExpiryMins"])),
                    Expires = DateTime.Now.AddMinutes(int.Parse(_configuration["BearerTokens:ExpiryMins"])),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        _configuration["BearerTokens:Key"])), SecurityAlgorithms.HmacSha256Signature)
                };
                var handler = new JwtSecurityTokenHandler();
                var securityToken = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

                return new { success = true, token = handler.WriteToken(securityToken) };
            }
            return new { success = false };
        }
    }
}
