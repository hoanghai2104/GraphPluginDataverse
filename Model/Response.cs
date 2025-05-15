using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Model
{
    public class Response
    {
        public class AzureAD
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
            public string access_token { get; set; }
        }

        public class Error
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }

        public class AccessResponse
        {
            public bool is_succeed { get; set; }
            public string access_token { get; set; }
            public string message { get; set; }
        }
    }
}
