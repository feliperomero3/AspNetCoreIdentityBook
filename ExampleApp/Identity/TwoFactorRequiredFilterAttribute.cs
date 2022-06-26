using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Identity
{
    public class TwoFactorRequiredFilterAttribute : Attribute, IAsyncPageFilter, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await ApplyPolicy(context.HttpContext);

            if (result is not null)
            {
                context.Result = result;
            }
            else
            {
                await next.Invoke();
            }
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var result = await ApplyPolicy(context.HttpContext);

            if (result is not null)
            {
                context.Result = result;
            }
            else
            {
                await next.Invoke();
            }
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        private static async Task<IActionResult> ApplyPolicy(HttpContext context)
        {
            var authService = context.RequestServices.GetService<IAuthorizationService>();

            var authorizationResult = (await authService.AuthorizeAsync(context.User, "Full2FARequired"));

            if (!authorizationResult.Succeeded)
            {
                var path = $"{context.Request.Path}{context.Request.QueryString}";

                return new RedirectToPageResult("TwoFactorAuthenticationRequired", new { returnUrl = path });
            }

            return null;
        }
    }
}