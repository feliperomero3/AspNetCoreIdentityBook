using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ExampleApp.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleApp.Pages
{
    public class ApiDataModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly HttpClient _httpClient;

        public ApiDataModel(UserManager<AppUser> userManager, HttpClient httpClient)
        {
            _userManager = userManager;
            _httpClient = httpClient;
        }

        public string Data { get; set; } = "No Data";

        // This method retrieves the authorization token produced by the demoAuth scheme and
        // uses it for the Authorization header in a request to the demonstration controller.
        // The response from the controller is parsed into JSON and used to set the value
        // of the Data property that is displayed by the view part of the page.
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            if (user is not null)
            {
                var token = await _userManager.GetAuthenticationTokenAsync(user, "demoAuth", "access_token");
                
                if (!string.IsNullOrEmpty(token))
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44324/api/DemoExternalApi");

                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await _httpClient.SendAsync(message);
                    
                    var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                    Data = document.RootElement.GetString("data");
                }
            }
        }
    }
}
