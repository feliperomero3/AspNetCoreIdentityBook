using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Test() => View();

        [Authorize(Roles = "User", AuthenticationSchemes = "OtherScheme")]
        public ActionResult Protected() => View("Test", "Protected Action");
    }
}
