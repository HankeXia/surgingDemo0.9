using Surging.Core.ProxyGenerator;
using surgingDemo.Data.Models;
using surgingDemo.IModuleServices.UserManager;
using surgingDemo.Modules.UserManager.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace surgingDemo.Modules.UserManager.Domain
{
    public class UserService : ProxyServiceBase, IUserService
    {
        private readonly UsersRepository _usersRepository;
        public UserService(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public Task<int> AddUser(Users users)
        {
            return _usersRepository.Add(users);
        }

        public Task<List<Users>> GetUsers()
        {
            return _usersRepository.GetUsers();
        }
    }
}
