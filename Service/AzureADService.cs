using GraphPlugin.Interface;
using GraphPlugin.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static GraphPlugin.Model.Response;

namespace GraphPlugin.Implement
{
    public class AzureADService: IAzureADService
    {
        public AccessResponse GetAccessToken()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AzureRegistrationApp.Endpoint);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(AzureRegistrationApp.Body);
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            string result = string.Empty;
            var resp = new AccessResponse();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                var obj = JsonSerializer.Deserialize<AzureAD>(result);

                resp = new AccessResponse()
                {
                    is_succeed = true,
                    access_token = obj.access_token,
                    message = "OK"
                };
            }
            catch (WebException ex)
            {
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
                var obj = JsonSerializer.Deserialize<Error>(result);

                resp = new AccessResponse()
                {
                    is_succeed = false,
                    message = obj.error_description
                };
            }

            return resp;
        }
    }
}
