using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlugin.Interface
{
    public interface IAzureADCredentialImplement
    {
        /// <summary>
        /// This method is used to get access token from Azure Registration App.
        /// </summary>
        /// <param name="serviceProvider"></param>
        void Execute(IServiceProvider serviceProvider);
    }
}
