using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GraphPlugin.Model.Response;

namespace GraphPlugin.Interface
{
    public interface IAzureADService
    {
        AccessResponse GetAccessToken(IOrganizationService service);
    }
}
