using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Model
{
    public class AzureRegistrationApp
    {
        private const string clientId = "b7333b20-4ae1-4e81-86ae-8b5f3116414c";
        private const string tenantId = "3b5ff753-4aa1-41c3-8a58-e148e35fa1ab";
        private const string secretKey = "Bzk8Q~BizYsDPQVnbfLthRX6q13M1OkyuiuklaWr";
        private const string scope = "https://graph.microsoft.com/.default";

        public static string Endpoint => $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
        public static string Body => $"client_id={Uri.EscapeDataString(clientId)}" +
                          $"&scope={Uri.EscapeDataString(scope)}" +
                          $"&client_secret={Uri.EscapeDataString(secretKey)}" +
                          $"&grant_type=client_credentials";
    }
}
