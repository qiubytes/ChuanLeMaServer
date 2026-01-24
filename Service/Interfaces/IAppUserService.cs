using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IAppUserService
    {
        public Task<List<AppUser>> All();
    }
}
