﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Test() => View();

        public ActionResult Protected() => View("Test", "Protected Action");

        [AllowAnonymous]
        public ActionResult Public() => View("Test", "Unauthenticated Action");
    }
}
