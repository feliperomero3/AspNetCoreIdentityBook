using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityApp.Pages.Identity
{
    public static class IdentityExtensions
    {
        public static void ProcessOperationResult(this IdentityResult result, ModelStateDictionary modelState)
        {
            foreach (IdentityError error in result.Errors ?? Enumerable.Empty<IdentityError>())
            {
                modelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
