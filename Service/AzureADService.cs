using GraphPlugin.Interface;
using GraphPlugin.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using static GraphPlugin.Model.DataverseEntity;
using static GraphPlugin.Model.Response;

namespace GraphPlugin.Implement
{
    public class AzureADService: IAzureADService
    {
        public AccessResponse GetAccessToken(IOrganizationService service)
        {
            try
            {
                EntityCollection credentials = GetCredential(service);
                var entities = credentials.Entities;
                bool IsAlreadyInitCredential = false;

                string accessToken = string.Empty;
                
                if (entities.Count() > 0)
                {
                    accessToken = entities[0].GetAttributeValue<string>(AzureADCredential.AccessToken);
                    IsAlreadyInitCredential = true;
                }
                   
                if (!IsExpiredToken(accessToken))
                    return new AccessResponse { is_succeed = true, access_token = accessToken, message = "success" };

                var obj = GetAccessToken();
                if (obj.is_succeed)
                {
                    if (IsAlreadyInitCredential)
                    {
                        //Update to dataverse entity
                        Entity updateCredential = new Entity(AzureADCredential.Id)
                        {
                            Id = entities[0].Id
                        };
                        updateCredential[AzureADCredential.AccessToken] = obj.access_token;
                        service.Update(updateCredential);
                    }
                    else 
                    {
                        Entity newCredential = new Entity(AzureADCredential.Id);
                        newCredential[AzureADCredential.AccessToken] = obj.access_token;

                        service.Create(newCredential);
                    }
                }

                return obj;
            }
            catch (Exception e)
            {
                return new AccessResponse() { is_succeed = false, message = e.Message };
            }
        }
        private AccessResponse GetAccessToken()
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
        private EntityCollection GetCredential(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression(AzureADCredential.Id)
            {
                TopCount = 1
            };
            query.ColumnSet.AddColumns(AzureADCredential.AccessToken);
            EntityCollection results = service.RetrieveMultiple(query);

            return results;
        }

        private bool IsExpiredToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return true;

            var parts = accessToken.Split('.');
            if (parts.Length != 3)
                throw new Exception("Invalid JWT token");

            string payload = parts[1];

            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
                case 1: payload += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(payload);
            string jsonPayload = Encoding.UTF8.GetString(bytes);

            var doc = JsonDocument.Parse(jsonPayload);
            if (!doc.RootElement.TryGetProperty("exp", out JsonElement expElement))
                throw new Exception("Root element cannot get JWT Token expiration datetime.");

            long expUnix = expElement.GetInt64();
            var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            //Check som hon 3 phut de renew
            return DateTime.UtcNow.AddMinutes(3) > exp;
        }
    }
}
