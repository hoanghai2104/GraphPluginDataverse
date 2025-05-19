using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GraphPlugin.Model.SharepointModel;

namespace GraphPlugin.Interface
{
    public interface IGraphAPIService
    {
        void GetItems(string endpoint, string accessToken, ref List<Item> items);
        UserInformation GetUserInformation(Guid azureADObjectId, string internalemail, string accessToken);
    }
}
