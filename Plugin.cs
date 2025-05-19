using GraphPlugin.Helper;
using GraphPlugin.Implement;
using GraphPlugin.Interface;
using GraphPlugin.Model;
using GraphPlugin.Service;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static GraphPlugin.Model.DataverseEntity;

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

    public class SharepointUser : IPlugin
    {
        private IPluginExecutionContext _context;
        private IOrganizationService _service;
        private readonly IGraphAPIService _graphAPIService;
        private readonly IAzureADService _azureADService;

        public SharepointUser(IGraphAPIService graphAPIService, IAzureADService azureADService)
        {
            _graphAPIService = graphAPIService;
            _azureADService = azureADService;
        }
        public SharepointUser() : this(new GraphAPIService(), new AzureADService())
        {
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ServiceProviderConfig.ConfigureServices(serviceProvider, out _service, out _context);

                var credential = _azureADService.GetAccessToken(_service);
                if (!credential.is_succeed)
                    throw new Exception(credential.message);

                var user = _service.Retrieve(SystemUser.Id, _context.UserId, new ColumnSet(SystemUser.AzureADObjectID, SystemUser.DomainName, SystemUser.InternalEmailAddress));
                string username = user.GetAttributeValue<string>(SystemUser.InternalEmailAddress);

                string errorMsg = $"User {username} has not been associated with SharePoint through Azure Active Directory.";

                if (!user.Contains(SystemUser.AzureADObjectID))
                    throw new Exception(errorMsg);

                Guid aadObjectId = user.GetAttributeValue<Guid>(SystemUser.AzureADObjectID);
                var userInfo = _graphAPIService.GetUserInformation(aadObjectId, username, credential.access_token);
                if (userInfo is null)
                    throw new Exception(errorMsg);

                _context.OutputParameters["nwv_userInfoId"] = userInfo.id;
                _context.OutputParameters["nwv_userInfoEmail"] = username;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            
        }
    }

    public class DOCAttributeValueSet : IPlugin
    {
        private readonly IDOCAttributeValueSetImplement _docAttributeValueSetImplement;

        public DOCAttributeValueSet(IDOCAttributeValueSetImplement docAttributeValueSetImplement)
        {
            _docAttributeValueSetImplement = docAttributeValueSetImplement;
        }
        public DOCAttributeValueSet() : this(new DOCAttributeValueSetImplement())
        {
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            _docAttributeValueSetImplement.Execute(serviceProvider);
        }
    }

    public class ExternalFunction: IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            
        }
    }

    public class TempTest: IPlugin
    {
        private IPluginExecutionContext _context;
        private IOrganizationService _service;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ServiceProviderConfig.ConfigureServices(serviceProvider, out _service, out _context);

                QueryExpression query = new QueryExpression("nwv_approvalsystemattributevaluesetdetail")
                {
                    ColumnSet = new ColumnSet("nwv_attributevaluesetname")
                };

                query.Criteria.AddCondition("nwv_attributegroupdetaildefinition", ConditionOperator.Equal, new Guid("35734852-2a20-f011-998a-002248566c34"));
                query.Criteria.AddCondition("nwv_attributevalue", ConditionOperator.Equal, new Guid("27d36184-6c31-f011-8c4e-6045bd1e417d"));

                EntityCollection results = _service.RetrieveMultiple(query);

                var setIds = new List<Guid>();
                var batchCollection = new EntityCollection();

                int eachBatchSize = 500;
                int batchSize = eachBatchSize;
                int count = 1;

                foreach (Entity record in results.Entities)
                {
                    EntityReference setRef = (EntityReference)record["nwv_attributevaluesetname"];
                    Guid setId = setRef.Id;

                    setIds.Add(setId);

                    if (count == batchSize || count == results.Entities.Count())
                    {
                        QueryExpression query1 = new QueryExpression("nwv_approvalsystemattributevaluesetdetail")
                        {
                            ColumnSet = new ColumnSet("nwv_attributevalue")
                        };
                        query1.Criteria.AddCondition("nwv_attributegroupdetaildefinition", ConditionOperator.Equal, new Guid("7ca78259-2a20-f011-998a-002248566c34"));
                        //query1.Criteria.AddCondition("nwv_attributevalue", ConditionOperator.Equal, new Guid("7054b074-6c31-f011-8c4e-6045bd1e417d"));
                        query1.Criteria.AddCondition("nwv_attributevaluesetname", ConditionOperator.In, setIds.Cast<object>().ToArray());
                        EntityCollection results1 = _service.RetrieveMultiple(query1);

                        batchCollection.Entities.AddRange(results1.Entities.ToList());

                        setIds.Clear();
                        eachBatchSize += 500;
                    }
                    count++;
                }

                var distinctEntities = batchCollection.Entities
                    .GroupBy(e => e.GetAttributeValue<EntityReference>("nwv_attributevaluesetname")?.Id)
                    .Select(g => g.First()).ToList();

                var entityCollection = new EntityCollection();
                entityCollection.Entities.AddRange(distinctEntities);
                _context.OutputParameters["nwv_OutputAttributeValueSetDetails"] = entityCollection;

                //_context.OutputParameters["nwv_tempstring"] = setId.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }

}
