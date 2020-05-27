using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDaemonApp.Configuration
{
    public class AzureAdSection
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CertificateName { get; set; }
    }
}
