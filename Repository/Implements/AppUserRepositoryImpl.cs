using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repository.Implements
{
    public class AppUserRepositoryImpl : IAppUserRepository
    {
        private readonly AppDbContext _dbcontext;
        public AppUserRepositoryImpl(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<AppUser>> All()
        {
            //return await _dbcontext.AppUsers.ToListAsync();

            //使用表达式树实现等同功能
            //参数表达式 
            var param = Expression.Parameter(typeof(AppUser), "x");
            //属性表达式
            var property = Expression.Property(param, "Id");
            //常量表达式
            var constant = Expression.Constant("-1");
            //二元表达式 
            var equal = Expression.Equal(property, constant);
            //lambda表达式
            var lambdaExp = Expression.Lambda<Func<AppUser, bool>>(equal, param);
            return await _dbcontext.AppUsers.Where(lambdaExp).ToListAsync();
        }

        public async Task<(bool, string, string)> Login(string username, string password)
        {
            AppUser? appUser = await _dbcontext.AppUsers.FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            if (appUser == null)
            {
                return (false, "用户名或者密码错误", "");
            }
            else
            {
                ///生成token
                string token = Guid.NewGuid().ToString();
                _dbcontext.AppUserTokens.Add(new AppUserToken
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = appUser.Id,
                    Token = token,
                    AddTime = DateTime.Now
                });
                await _dbcontext.SaveChangesAsync();
                return (true, "登录成功", token);
            }
        }

        public Task<(bool, string)> Register(AppUser appUser)
        {
            if (_dbcontext.AppUsers.Any(u => u.UserName == appUser.UserName))
            {
                return Task.FromResult((false, "用户名已存在"));
            }
            else
            {
                _dbcontext.AppUsers.Add(appUser);
                _dbcontext.SaveChanges();
                return Task.FromResult((true, "注册成功"));
            }
        }
    }
}
