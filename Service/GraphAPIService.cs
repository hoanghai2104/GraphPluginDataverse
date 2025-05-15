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
        public void GetItems(string endpoint, string accessToken, ref List<Item> items)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers["Authorization"] = "Bearer " + accessToken;
                request.Headers["Prefer"] = "HonorNonIndexedQueriesWarningMayFailRandomly";

                string result;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                var res = JsonSerializer.Deserialize<ItemResponse>(result);
                items.AddRange(res.value);

                if (res.odataNextLink != null && res.odataNextLink != string.Empty)
                    GetItems(res.odataNextLink, accessToken, ref items);
            }
            catch(WebException wex)
            {
                throw new InvalidPluginExecutionException(wex.Message);
            }
        }
    }
}
