using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Model
{
    public class SharepointEnv
    {
        private const string graphUrl = "https://graph.microsoft.com/v1.0";
        private const string siteId = "thenaviworldgroupltd.sharepoint.com,b619f896-3fa9-42d0-b142-354772e92a7c,d042b3ba-6ff1-4334-9d49-9b6be8a3213c";
        private const string paperlessDriveId = "b!lvgZtqk_0EKxQjVHcukqfLqzQtDxbzRDnUmba-ijITwIJUb-_F2IRKaJT7srYUTA";
        private const string draftDriveName = "Draft";
        private const string paperlessListId = "fe462508-5dfc-4488-a689-4fbb2b6144c0";

        public static string DRAFT_DRIVE_ENDPOINT => $"{graphUrl}/drives/{paperlessDriveId}/root:/{draftDriveName}/";
        public static string PAPERLESS_DRIVE_ENDPOINT => $"{graphUrl}/drives/{paperlessDriveId}/root:/";
        public static string PAPERLESS_ITEMS_ENDPOINT => $"{graphUrl}/sites/{siteId}/lists/{paperlessListId}/items?expand=fields(select=*,FileRef,FileDirRef)";
        public static string USER_INFO_ENDPOINT => $"{graphUrl}/sites/{siteId}/lists/User Information List/items";
    }
}
