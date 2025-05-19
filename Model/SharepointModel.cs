using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphPlugin.Model
{
    public class SharepointModel
    {
        public class DriveResponse
        {
            [JsonPropertyName("@odata.context")]
            public string odataContext { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string odataNextLink { get; set; }
            public List<Drive> value { get; set; }
        }
        public class ItemResponse
        {
            [JsonPropertyName("@odata.context")]
            public string odataContext { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string odataNextLink { get; set; }
            public List<Item> value { get; set; }
        }
        public class Drive
        {
            public string id { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public Folder folder { get; set; }
            public File file { get; set; }
            public FileSystemInfo fileSystemInfo { get; set; }
            public int size { get; set; }
            public DateTime createdDateTime { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public DateTime createdDateTime { get; set; }
            public string webUrl { get; set; }
            public ContentType contentType { get; set; }
            public Fields fields { get; set; }
        }

        public class ContentType
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Fields
        {
            [JsonPropertyName("@odata.etag")]
            public string odataEtag { get; set; }
            public string id { get; set; }
            public string ContentType { get; set; }
            public string FileLeafRef { get; set; }
            public string FileRef { get; set; }
            public string FileDirRef { get; set; }
            public string Title { get; set; }
            public string AttributeValueSet { get; set; }
            //public List<string> HVBB_x0020_Keywords { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
            public string DocIcon { get; set; }
            public string FileSizeDisplay { get; set; }
            public string AuthorLookupId { get; set; }
            public string Author0LookupId { get; set; }
            public string EditorLookupId { get; set; }
            public string _UIVersionString { get; set; }

        }
        public class Folder
        {
            public int childCount { get; set; }
        }

        public class File
        {
            public Hashes hashes { get; set; }
            public string mimeType { get; set; }

            public class Hashes
            {
                public string quickXorHash { get; set; }
            }
        }

        public class FileSystemInfo
        {
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
        }


        public class UserInformationResponse
        {
            [JsonPropertyName("@odata.context")]
            public string odataContext { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string odataNextLink { get; set; }
            public List<UserInformation> value { get; set; }
        }

        public class UserInformation
        {
            public string id { get; set; }
            public CreatedBy createdBy { get; set; }
            public Fields fields { get; set; }

            public class CreatedBy
            {
                public User user { get; set; }
                public class User
                {
                    public string email { get; set; }
                    public string id { get; set; }//AzureAD Object ID
                    public string displayName { get; set; }
                }
            }
            public class Fields
            {
                public string Title { get; set; }
                public string EMail { get; set; }
                public string UserName { get; set; }
                public bool Deleted { get; set; }
            }
        }
    }

}
