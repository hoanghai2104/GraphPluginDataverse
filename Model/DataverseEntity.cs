using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Model
{
    public static class DataverseEntity
    {
        public static class AzureADCredential
        {
            public static string Id => "nwv_azureadcredential";
            public static string AccessToken => "nwv_accesstoken";
        }
        public static class SystemUser
        {
            public static string Id => "systemuser";
            public static string DomainName => "domainname";
            public static string InternalEmailAddress => "internalemailaddress";
            public static string AzureADObjectID => "azureactivedirectoryobjectid";
        }
        public static class AttributeValueSetDetail
        {
            public static string Id => "nwv_approvalsystemattributevaluesetdetailid";
            public static string Name => "nwv_approvalsystemattributevaluesetdetail";
            public static string AttributeGroupDetailDefinition => "nwv_attributegroupdetaildefinition";//LevelId
            public static string AttributeValue => "nwv_attributevalue";
            public static string AttributeValueSetName => "nwv_attributevaluesetname";
            
        }
    }
}
