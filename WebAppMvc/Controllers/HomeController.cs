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
            //using (var httpClient = new HttpClient())
            //{
            //    using (var response = await httpClient.GetAsync("https://localhost:44389/weatherforecast"))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        var apiOutput = JsonConvert.DeserializeObject<List<WeatherForecast>>(apiResponse);
            //        return View(apiOutput);
            //    }
            //}

            //// Acquire the access token.
            string[] scopes = new string[] { "user.read", "api://5e971e5c-a661-4d82-ba97-935480492129/access_as_user" };
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            string url = "https://localhost:44389/weatherforecast";

            // Use the access token to call a protected web API.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string json = await client.GetStringAsync(url);
            var apiOutput = JsonConvert.DeserializeObject<List<WeatherForecast>>(json);
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
