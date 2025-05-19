using GraphPlugin.Implement;
using GraphPlugin.Interface;
using GraphPlugin.Model;
using Microsoft.SqlServer.Server;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static GraphPlugin.Model.SharepointModel;

namespace GraphPlugin.Service
{
    public class GraphAPIService : IGraphAPIService
    {
        private const string NONINDEXED_PREFER = "HonorNonIndexedQueriesWarningMayFailRandomly";
        public void GetItems(string endpoint, string accessToken, ref List<Item> items)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers["Authorization"] = "Bearer " + accessToken;
                request.Headers["Prefer"] = NONINDEXED_PREFER;

                string result;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                var res = JsonSerializer.Deserialize<ItemResponse>(result);
                var arr = res.value.FindAll(p => p.fields.FileDirRef.Contains("/sites/HVBBPaperlessDEV/Paperless/HVN"));

                items.AddRange(arr);

                if (res.odataNextLink != null && res.odataNextLink != string.Empty)
                    GetItems(res.odataNextLink, accessToken, ref items);
            }
            catch(WebException wex)
            {
                throw new InvalidPluginExecutionException(wex.Message);
            }
        }

        public UserInformation GetUserInformation(Guid azureADObjectId, string internalemail, string accessToken)
        {
            try
            {
                string endpoint = SharepointEnv.USER_INFO_ENDPOINT + $"?expand=fields&$filter=fields/UserName eq '{internalemail}'";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers["Authorization"] = "Bearer " + accessToken;
                request.Headers["Prefer"] = NONINDEXED_PREFER;

                string result;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                var res = JsonSerializer.Deserialize<UserInformationResponse>(result);
                var users = res.value;

                var user = users.Find(p => p.createdBy.user.id.Equals(azureADObjectId.ToString()));

                if (user != null)
                {
                    //Validate if user is deleted.
                    if (user.fields.Deleted)
                        throw new Exception($"User {internalemail} has been deleted.");
                }

                return user;
            }
            catch (WebException wex)
            {
                throw new InvalidPluginExecutionException(wex.Message);
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
