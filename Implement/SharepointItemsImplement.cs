using GraphPlugin.Helper;
using GraphPlugin.Interface;
using GraphPlugin.Model;
using GraphPlugin.Service;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text.Json;
using static GraphPlugin.Model.SharepointModel;

namespace GraphPlugin.Implement
{
    public class SharepointItemsImplement : ISharepointItemsImplement
    {
        private IOrganizationService _service;
        private IPluginExecutionContext _context;
        private ITracingService _tracingService;

        private readonly IAzureADService _azureService;
        private readonly IGraphAPIService _graphAPIService;
        public SharepointItemsImplement(IAzureADService azureService, IGraphAPIService graphAPIService)
        {
            _azureService = azureService;
            _graphAPIService = graphAPIService;
        }
        public SharepointItemsImplement() : this(new AzureADService(), new GraphAPIService())
        {
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            ServiceProviderConfig.ConfigureServices(serviceProvider, out _tracingService, out _service, out _context);

            var aad = _azureService.GetAccessToken(_service);
            if (!aad.is_succeed)
                throw new InvalidPluginExecutionException(aad.message);

            string filter = _context.InputParameters["nwv_filter"].ToString();
            string userInfoId = _context.InputParameters["nwv_userinfoid"].ToString();

            if (string.IsNullOrEmpty(filter))
                throw new InvalidPluginExecutionException("Filter must be added to get items in Sharepoint List.");

            filter += $" and fields/Author0LookupId eq '{userInfoId}'";

            var items = new List<Item>();
            _graphAPIService.GetItems(SharepointEnv.PAPERLESS_ITEMS_ENDPOINT + "&" + filter, aad.access_token, ref items);

            //Response
            _context.OutputParameters["nwv_items"] = JsonSerializer.Serialize(items);
        }
    }
}
