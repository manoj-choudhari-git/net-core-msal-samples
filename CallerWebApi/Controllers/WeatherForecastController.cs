using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json;

namespace CallerWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly string[] scopeRequiredByApi = new string[] { "caller-api" };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return await CallOtherApi();
        }

        private async Task<dynamic> CallOtherApi()
        {

            // scopes required to access the new weather forecast service
            string[] scopes = new string[] { "user.read", "api://5e999e55-a661-4982-b897-965480492129/access_as_user" };

            // Get cached token
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

            HttpClient client = new HttpClient();

            // After the token has been returned by Microsoft Authentication Library (MSAL), 
            // add it to the HTTP authorization header before making the call 
            // to access the weather forecast service.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Call the To Do list service.
            string url = "https://localhost:44389/weatherforecast";

            HttpResponseMessage response = await client.GetAsync(url);

            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<WeatherForecast>>(content);
            }
            

            throw new Exception("api call failed");
        }
    }
}
