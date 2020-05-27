using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebAPI;
using Microsoft.Identity.Web.UI;
using System.Net.Http.Headers;
using Microsoft.Identity.Web;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;

        public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public void OnGet()
        {

        }

        public async Task OnPostRaiseApiCall()
        {
            //// Acquire the access token.
            string[] scopes = new string[] { "user.read", "api://ffff3e7b-20cb-42cb-8a23-2d6c8003ee3a/caller-api" };
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            ///string url = "https://localhost:44389/weatherforecast";
            string url = "https://localhost:44370/weatherforecast";

            // Use the access token to call a protected web API.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string json = await client.GetStringAsync(url);
            var apiOutput = JsonConvert.DeserializeObject<List<WeatherForecast>>(json);
            ViewData["ApiOutput"] = apiOutput;
        }
    }
}
