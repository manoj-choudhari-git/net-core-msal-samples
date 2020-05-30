using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B2CWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string accountIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (accountIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;
            try
            {
                ResultText.Text = "";
                authResult = await (app as PublicClientApplication).AcquireTokenInteractive(App.Scopes)
                    .ExecuteAsync();

                DisplayUserInfo(authResult);
                UpdateSignInState(true);
            }
            catch (MsalException ex)
            {
                try
                {
                    if (ex.Message.Contains("AADB2C90118"))
                    {
                        authResult = await (app as PublicClientApplication).AcquireTokenInteractive(App.Scopes)
                            .WithPrompt(Prompt.SelectAccount)
                            .WithB2CAuthority(App.AuthorityResetPassword)
                            .ExecuteAsync();
                    }
                    else
                    {
                        ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
                    }
                }
                catch (Exception exe)
                {
                    ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{exe}";
                }
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
            }
        }


        private async void CallApiButton_Click(object sender, RoutedEventArgs e)
        {
            //// Try to Acquire token silently
            //// If not possible, then try to acquire it interactively.
            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;
            IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();
            try
            {
                authResult = await app.AcquireTokenSilent(App.ApiScopes, GetAccountByPolicy(accounts, App.PolicySignUpSignIn))
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. 
                // This indicates you need to call AcquireTokenAsync to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(App.Scopes)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
                return;
            }

            //// After token is available
            //// Call the API and Show the result
            if (authResult != null)
            {
                if (string.IsNullOrEmpty(authResult.AccessToken))
                {
                    ResultText.Text = "Access token is null (could be expired). Please do interactive log-in again.";
                }
                else
                {
                    ResultText.Text = await GetHttpContentWithToken(App.ApiEndpoint, authResult.AccessToken);
                    DisplayUserInfo(authResult);
                }
            }
        }

        /// <summary>
        /// Perform an HTTP GET request to a URL using an HTTP Authorization header
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="token">The token</param>
        /// <returns>String containing the results of the GET operation</returns>
        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();
            try
            {
                while (accounts.Any())
                {
                    await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await App.PublicClientApp.GetAccountsAsync();
                }

                UpdateSignInState(false);
            }
            catch (MsalException ex)
            {
                ResultText.Text = $"Error signing-out user: {ex.Message}";
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = App.PublicClientApp;
                IEnumerable<IAccount> accounts = await App.PublicClientApp.GetAccountsAsync();

                AuthenticationResult authResult = await app.AcquireTokenSilent(App.Scopes,
                                                                               GetAccountByPolicy(accounts, App.PolicySignUpSignIn))
                    .ExecuteAsync();

                DisplayUserInfo(authResult);
                UpdateSignInState(true);
            }
            catch (MsalUiRequiredException)
            {
                // Ignore, user will need to sign in interactively.
                ResultText.Text = "You need to sign-in first, and then Call API";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
            }
        }

        private void UpdateSignInState(bool signedIn)
        {
            if (signedIn)
            {
                SignOutButton.Visibility = Visibility.Visible;

                SignInButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ResultText.Text = "";
                TokenInfoText.Text = "";

                SignOutButton.Visibility = Visibility.Collapsed;
                SignInButton.Visibility = Visibility.Visible;
            }
        }

        private void DisplayUserInfo(AuthenticationResult authResult)
        {
            TokenInfoText.Text = "";

            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtInput = authResult.IdToken;

            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken != true)
            {
                TokenInfoText.Text = "The token doesn't seem to be in a proper JWT format.";
            }
            if (readableToken == true)
            {
                var token = jwtHandler.ReadJwtToken(jwtInput);

                //Extract the headers of the JWT
                var headers = token.Header;
                var jwtHeader = "{";
                foreach (var h in headers)
                {
                    jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                }
                jwtHeader += "}";
                TokenInfoText.Text = "Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented);

                //Extract the payload of the JWT
                var claims = token.Claims;
                var jwtPayload = "{";
                foreach (Claim c in claims)
                {
                    jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
                }
                jwtPayload += "}";
                TokenInfoText.Text += "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);
            }

        }

        ////private string Base64UrlDecode(string s)
        ////{
        ////    s = s.Replace('-', '+').Replace('_', '/');
        ////    s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        ////    var byteArray = Convert.FromBase64String(s);
        ////    var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
        ////    return decoded;
        ////}
    }
}
