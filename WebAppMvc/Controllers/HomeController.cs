using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using WebAPI;
using WebAppMvc.Models;

namespace WebAppMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;

        public HomeController(ILogger<HomeController> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<IActionResult> Index()
        {
            //// Acquire the access token.
            string[] scopes = new string[] { "api://5e999e55-a661-4982-b897-965480492129/access_as_user" };
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

            // Use the access token to call a protected web API.
            HttpClient client = new HttpClient();
            string url = "https://localhost:44389/weatherforecast";
            
            // Set Bearer Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Call API
            string json = await client.GetStringAsync(url);
            var apiOutput = JsonConvert.DeserializeObject<List<WeatherForecast>>(json);

            // Send Data To View
            return View(apiOutput);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
