using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace B2CWPFApp.Extensions
{
    internal class EmbeddedCustomWebUI : ICustomWebUi
    {
        public const int DefaultWindowWidth = 450;
        public const int DefaultWindowHeight = 600;

        private readonly Window _windowOwner;
        private readonly string _windowTitle;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly WindowStartupLocation _windowStartupLocation;

        public EmbeddedCustomWebUI(Window owner,
            string title = "Sign in",
            int windowWidth = DefaultWindowWidth,
            int windowHeight = DefaultWindowHeight,
            WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner)
        {
            _windowOwner = owner ?? throw new ArgumentNullException(nameof(owner));
            _windowTitle = title;
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _windowStartupLocation = windowStartupLocation;
        }

        public Task<Uri> AcquireAuthorizationCodeAsync(Uri authorizationUri, Uri redirectUri, CancellationToken cancellationToken)
        {
            var taskCompletion = new TaskCompletionSource<Uri>();
            _windowOwner.Dispatcher.Invoke(() =>
            {
                new CustomLoginWindow(authorizationUri, redirectUri, taskCompletion, cancellationToken)
                {
                    Owner = _windowOwner,
                    Title = _windowTitle,
                    Width = _windowWidth,
                    Height = _windowHeight,
                    WindowStartupLocation = _windowStartupLocation,
                }.ShowDialog();
            });

            return taskCompletion.Task;
        }
    }
}
