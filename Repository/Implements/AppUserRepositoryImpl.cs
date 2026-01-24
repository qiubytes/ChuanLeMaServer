using Microsoft.EntityFrameworkCore;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
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
            return await _dbcontext.AppUsers.ToListAsync();
        }
    }
}
