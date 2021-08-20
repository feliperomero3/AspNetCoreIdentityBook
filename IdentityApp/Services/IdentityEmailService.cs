using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;

namespace IdentityApp.Services
{
    public class IdentityEmailService
    {
        public IdentityEmailService(IEmailSender sender,
            UserManager<IdentityUser> userMgr,
            IHttpContextAccessor contextAccessor,
            LinkGenerator generator,
            TokenUrlEncoderService encoder)
        {
            EmailSender = sender;
            UserManager = userMgr;
            ContextAccessor = contextAccessor;
            LinkGenerator = generator;
            TokenEncoder = encoder;
        }

        public IEmailSender EmailSender { get; set; }
        public UserManager<IdentityUser> UserManager { get; set; }
        public IHttpContextAccessor ContextAccessor { get; set; }
        public LinkGenerator LinkGenerator { get; set; }
        public TokenUrlEncoderService TokenEncoder { get; set; }

        private string GetUrl(string emailAddress, string token, string page)
        {
            string safeToken = TokenEncoder.EncodeToken(token);

            var values = new
            {
                email = emailAddress,
                token = safeToken
            };

            return LinkGenerator.GetUriByPage(ContextAccessor.HttpContext, page, handler: null, values: values);
        }
    }
}
