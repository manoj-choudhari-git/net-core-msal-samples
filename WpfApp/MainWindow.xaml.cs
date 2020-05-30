using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp.TokenCache;

namespace WpfApp
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

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string[] scopes = new string[] { "user.read", "api://5e999e55-a661-4982-b897-965480492129/access_as_user" };

            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;
            try
            {

                ResultText.Text = "";

                authResult = await (app as PublicClientApplication).AcquireTokenInteractive(scopes)
                    .ExecuteAsync();

                DisplayBasicTokenInfo(authResult);
                UpdateSignInState(true);
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error Acquiring Token:{Environment.NewLine}{ex}";
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

        private void DisplayBasicTokenInfo(AuthenticationResult authResult)
        {
            TokenInfoText.Text = "";
            if (authResult != null)
            {
                TokenInfoText.Text += $"Username: {authResult.Account.Username}" + Environment.NewLine;
                TokenInfoText.Text += $"Token Expires: {authResult.ExpiresOn.ToLocalTime()}" + Environment.NewLine;
            }
        }

        private async void CallAPIButton_Click(object sender, RoutedEventArgs e)
        {
            string[] scopes = new string[] { "api://5e999e55-a661-4982-b897-965480492129/access_as_user" };

            // This shows simplest version assuming there will not be errors 
            // while getting token silently.  If there are, token should be acquired 
            // using interactive API
            var accountList = await App.PublicClientApp.GetAccountsAsync();
            var authResult = await App.PublicClientApp
                            .AcquireTokenSilent(scopes, accountList.FirstOrDefault())
                            .ExecuteAsync();

            string accessToken = authResult.AccessToken;

            string url = "https://localhost:44389/weatherforecast";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string json = await client.GetStringAsync(url);
            ResultText.Text = json;
        }
    }
}
