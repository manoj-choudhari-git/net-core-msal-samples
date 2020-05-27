using Newtonsoft.Json;
using SampleDaemonApp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SampleDaemonApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var result = await DaemonAppBootstrapper.BootstrapAsync(ConfigurationManager.Current.Api.Scopes);
                await ProcessAsync(ConfigurationManager.Current.Api.BaseUrl, result.AccessToken);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Daemon App Failed : {ex}");
                Console.ResetColor();
            }
        }


        private static async Task ProcessAsync(string webApiUrl, string accessToken)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(accessToken))
            {
                var defaultRequetHeaders = client.DefaultRequestHeaders;
                if (defaultRequetHeaders.Accept == null || !defaultRequetHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                defaultRequetHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                HttpResponseMessage response = await client.GetAsync(webApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("================================================");
                    Console.WriteLine("API Output:");
                    Console.WriteLine(json);
                    Console.WriteLine("================================================");
                    Console.ResetColor();
                    return;
                }
            }

            throw new Exception("API Call Failed");
        }

    }
}
