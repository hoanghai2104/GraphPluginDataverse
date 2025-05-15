using GraphPlugin.Implement;
using GraphPlugin.Interface;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin
{
    public class AzureADCredentials: IPlugin
    {
        private readonly IAzureADCredentialImplement _azureADCredentialImplement;

        public AzureADCredentials(IAzureADCredentialImplement azureADCredentialImplement)
        {
            _azureADCredentialImplement = azureADCredentialImplement;
        }
        public AzureADCredentials() : this(new AzureADCredentialImplement())
        {
        }

        /// <summary>
        /// This method is used to get access token from Azure Registration App.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            _azureADCredentialImplement.Execute(serviceProvider);
        }
    }

    public class SharepointItems : IPlugin
    {
        private readonly ISharepointItemsImplement _sharepointItemsImplement;

        public SharepointItems(ISharepointItemsImplement sharepointItemsImplement)
        {
            _sharepointItemsImplement = sharepointItemsImplement;
        }
        public SharepointItems() : this(new SharepointItemsImplement())
        {
        }

        /// <summary>
        /// This method is used to get list items in Sharepoint by Graph API.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            _sharepointItemsImplement.Execute(serviceProvider);
        }
    }
}
