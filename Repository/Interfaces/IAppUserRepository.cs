using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Interfaces
{
    public interface IAppUserRepository
    {
        public Task<List<AppUser>> All();
        /// <summary>
        /// 用户登录 返回 (bool 是否成功, string 消息, string 令牌)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<(bool, string, string)> Login(string username, string password);
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        public Task<(bool,string)> Register(AppUser appUser);
    }
}
