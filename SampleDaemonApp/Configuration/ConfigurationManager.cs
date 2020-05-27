using Microsoft.Extensions.Configuration;
using System.IO;


namespace SampleDaemonApp.Configuration
{
    class ConfigurationManager
    {
        private static ConfigurationManager currentInstance = null;

        private ConfigurationManager()
        {
        }

        public AzureAdSection AzureAd { get; set; }

        public ApiSection Api { get; set; }


        public static ConfigurationManager Current
        {
            get
            {
                if (currentInstance == null)
                {
                    currentInstance = new ConfigurationManager();
                    currentInstance.Initialize("appsettings.json");
                }

                return currentInstance;
            }
        }

        private void Initialize(string path)
        {
            IConfigurationRoot Configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            Configuration = builder.Build();
            this.AzureAd = Configuration.GetSection("AzureAd").Get<AzureAdSection>();
            this.Api = Configuration.GetSection("Api").Get<ApiSection>();

        }
    }
}
