using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDaemonApp.Configuration
{
    public class ApiSection
    {
        public string BaseUrl { get; set; }

        public string[] Scopes { get; set; }
    }
}