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

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task OnPostRaiseApiCall()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44389/weatherforecast"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var apiOutput = JsonConvert.DeserializeObject<List<WeatherForecast>>(apiResponse);
                    ViewData["ApiOutput"] = apiOutput;
                }
            }
        }
    }
}
