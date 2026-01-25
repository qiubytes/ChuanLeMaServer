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
    }
}
