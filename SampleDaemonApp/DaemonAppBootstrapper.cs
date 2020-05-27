using Microsoft.Identity.Client;
using SampleDaemonApp.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SampleDaemonApp
{
    public static class DaemonAppBootstrapper
    {
        public static async Task<AuthenticationResult> BootstrapAsync(string[] scopes)
        {
            if (scopes == null || scopes.Length == 0)
            {
                throw new InvalidOperationException("scopes cannot be empty");
            }

            var app = CreateConfidentialClientApplication();
            return await Authenticate(app, scopes);
        }


        private static async Task<AuthenticationResult> Authenticate(IConfidentialClientApplication app, string[] scopes)
        {
            AuthenticationResult result = null;

            try
            {
                result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                Console.WriteLine("Token acquired successfully");
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Scope provided is not supported");
                Console.ResetColor();
            }

            return result;
        }


        private static IConfidentialClientApplication CreateConfidentialClientApplication()
        {
            var config = ConfigurationManager.Current.AzureAd;

            bool isClientSecretUsed = ValidateConfigurations();
            if (!isClientSecretUsed)
            {
                var configuredCertificate = ReadCertificate(config.CertificateName);
            }

            IConfidentialClientApplication app;

            if (isClientSecretUsed)
            {
                app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithClientSecret(config.ClientSecret)
                    .WithAuthority(new Uri(config.Authority))
                    .Build();
            }
            else
            {
                X509Certificate2 certificate = ReadCertificate(config.CertificateName);
                app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithCertificate(certificate)
                    .WithAuthority(new Uri(config.Authority))
                    .Build();
            }

            return app;
        }

        private static bool ValidateConfigurations()
        {
            var azureAd = ConfigurationManager.Current.AzureAd;

            if (azureAd == null)
            {
                throw new InvalidOperationException("Configuration section not added");
            }

            if (string.IsNullOrEmpty(azureAd.CertificateName?.Trim()) && string.IsNullOrEmpty(azureAd.ClientSecret?.Trim()))
            {
                throw new InvalidOperationException("Either certificate or client secret should be specified.  Both cannot be empty.");
            }

            return !string.IsNullOrEmpty(azureAd.ClientSecret?.Trim());
        }


        private static X509Certificate2 ReadCertificate(string certificateName)
        {
            if (string.IsNullOrWhiteSpace(certificateName))
            {
                throw new ArgumentException("certificateName should not be empty. Please set the CertificateName setting in the appsettings.json", "certificateName");
            }

            X509Certificate2 cert = null;

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = store.Certificates;

                // Find unexpired certificates.
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

                // From the collection of unexpired certificates, find the ones with the correct name.
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certificateName, false);

                // Return the first certificate in the collection, has the right name and is current.
                cert = signingCert.OfType<X509Certificate2>().OrderByDescending(c => c.NotBefore).FirstOrDefault();
            }

            return cert;
        }
    }
}
