using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Helper
{
    public static class ServiceProviderConfig
    {
        public static void ConfigureServices(IServiceProvider serviceProvider, out ITracingService tracingService, out IOrganizationService service, out IPluginExecutionContext context)
        {
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            service = serviceFactory.CreateOrganizationService(context.UserId);
        }
    }
}
