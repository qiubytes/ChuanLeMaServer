using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Interfaces
{
    public interface IAppUserRepository
    {
        public Task<List<AppUser>> All();
    }
}
