using GraphPlugin.Helper;
using GraphPlugin.Interface;
using GraphPlugin.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Implement
{
    public class AzureADCredentialImplement : IAzureADCredentialImplement
    {
        private IOrganizationService _service;
        private IPluginExecutionContext _context;
        private ITracingService _tracingService;
        private readonly IAzureADService _azureService;
        public AzureADCredentialImplement(IAzureADService azureService)
        {
            _azureService = azureService;
        }
        public AzureADCredentialImplement() : this(new AzureADService())
        {
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            ServiceProviderConfig.ConfigureServices(serviceProvider, out _tracingService, out _service, out _context);

            var aad = _azureService.GetAccessToken();
            _context.OutputParameters["nwv_isSucceed"] = aad.is_succeed;

            if (!aad.is_succeed)
            {
                _context.OutputParameters["nwv_message"] = aad.message;
                return;
            }

            _context.OutputParameters["nwv_message"] = "Sucess";
            _context.OutputParameters["nwv_accessToken"] = aad.access_token;
            _context.OutputParameters["nwv_draftDriveEndpoint"] = SharepointEnv.DRAFT_DRIVE_ENDPOINT;
            _context.OutputParameters["nwv_paperlessItemEndpoint"] = SharepointEnv.PAPERLESS_ITEMS_ENDPOINT;
        }
    }
}
