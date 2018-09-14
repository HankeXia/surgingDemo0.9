﻿using Microsoft.EntityFrameworkCore;
using Surging.Core.CPlatform.Ioc;
using surgingDemo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surgingDemo.Modules.UserManager.Repositories
{
    public class UsersRepository : BaseRepository
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Task<int> Add(Users users)
        {
            using (var db = new testContext())
            {
                db.Users.Add(users);
                return db.SaveChangesAsync();
            }
        }
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public Task<List<Users>> GetUsers()
        {
            using (var db = new testContext())
            {
                return db.Users.AsNoTracking().ToListAsync();
            }
        }
    }
}
