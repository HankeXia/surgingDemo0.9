using Surging.Core.CPlatform.Ioc;
using Surging.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Surging.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Surging.Core.CPlatform.Support.Attributes;
using surgingDemo.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace surgingDemo.IModuleServices.UserManager
{
    [ServiceBundle("api/{Service}")]
    public interface IUserService : IServiceKey
    {

        [Command(ShuntStrategy = AddressSelectorMode.HashAlgorithm)]
        Task<List<Users>> GetUsers();
        [Command(ShuntStrategy = AddressSelectorMode.HashAlgorithm)]
        Task<int> AddUser(Users users);
    }
}
