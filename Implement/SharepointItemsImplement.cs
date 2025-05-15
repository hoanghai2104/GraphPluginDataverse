using GraphPlugin.Helper;
using GraphPlugin.Interface;
using GraphPlugin.Model;
using GraphPlugin.Service;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var aad = _azureService.GetAccessToken();
            if (!aad.is_succeed)
                throw new InvalidPluginExecutionException(aad.message);

            var items = new List<Item>();
            string endpoint = SharepointEnv.PAPERLESS_ITEMS_ENDPOINT + "&$filter=fields/ContentType ne 'Folder'";
            _graphAPIService.GetItems(endpoint, aad.access_token, ref items);
        }
    }
}
