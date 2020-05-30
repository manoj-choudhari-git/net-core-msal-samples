using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B2CWPFApp
{
    /// <summary>
    /// Interaction logic for CustomLoginWindow.xaml
    /// </summary>
    public partial class CustomLoginWindow : Window
    {
        private readonly Uri _authorizeUri;
        private readonly Uri _redirectUri;
        private readonly TaskCompletionSource<Uri> _taskCompletionSource;
        private readonly CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
     
        public CustomLoginWindow(
            Uri authorizationUri,
            Uri redirectUri,
            TaskCompletionSource<Uri> taskCompletionSource,
            CancellationToken cancellationToken)
        {
            InitializeComponent();
            _authorizeUri = authorizationUri;
            _redirectUri = redirectUri;
            _taskCompletionSource = taskCompletionSource;
            _cancellationToken = cancellationToken;
        }



        private void webBrowserControl_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // if URI does not starts with redirect URI, then return
            if (!e.Uri.ToString().StartsWith(_redirectUri.ToString()))
            {
                // not redirect uri case
                return;
            }

            // Parse query strings and check for code parameter
            var query = HttpUtility.ParseQueryString(e.Uri.Query);
            if (query.AllKeys.Any(x => x == "code"))
            {
                // It has a code parameter.
                _taskCompletionSource.SetResult(e.Uri);
                Close();
            }


            // Otherwise there is an error
            _taskCompletionSource.SetException(
                new MsalExtensionException(
                    $"An error occurred, error: {query.Get("error")}, error_description: {query.Get("error_description")}"));

            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register cancellation token
            _cancellationTokenRegistration = _cancellationToken.Register(() => _taskCompletionSource.SetCanceled());
        
            // Navigate to entry point of authorization flow
            webBrowserControl.Navigate(_authorizeUri);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Displose the cancellation token
            _taskCompletionSource.TrySetCanceled();
            _cancellationTokenRegistration.Dispose();
        }
    }
}
