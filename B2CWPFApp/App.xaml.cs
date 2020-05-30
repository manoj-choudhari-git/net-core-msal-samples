using B2CWPFApp.TokenCache;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace B2CWPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string Tenant = "samplead.onmicrosoft.com";
        private static readonly string AzureAdB2CHostname = "samplead.b2clogin.com";
        private static readonly string ClientId = "888fff1d-16c3-4de6-92af-3d4ab54a860a";
        private static readonly string RedirectUri = "http://localhost";
        public static string PolicySignUpSignIn = "B2C_1_SignUpSignIn";
        public static string PolicyEditProfile = "B2C_1_Edit_Profile";
        public static string PolicyResetPassword = "B2C_1_Pwd_Reset";

        public static string ApiEndpoint = "https://localhost:44379/weatherforecast";
        public static string[] ApiScopes = { "https://samplead.onmicrosoft.com/sample-api/api-scope" };
        public static string[] Scopes = { "openid", "profile", "https://samplead.onmicrosoft.com/sample-api/api-scope" };

        private static string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignUpSignIn = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";

        public static IPublicClientApplication PublicClientApp { get; private set; }

        static App()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(AuthoritySignUpSignIn)
                .WithRedirectUri(RedirectUri)
                .WithLogging(Log, LogLevel.Verbose, false) //PiiEnabled set to false
                .Build();

            TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
        }
        private static void Log(LogLevel level, string message, bool containsPii)
        {
            string logs = ($"{level} {message}");
            StringBuilder sb = new StringBuilder();
            sb.Append(logs);
            File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalLogs.txt", sb.ToString());
            sb.Clear();
        }
    }
}
