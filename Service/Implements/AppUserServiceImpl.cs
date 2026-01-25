using Model;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Implements
{
    public class AppUserServiceImpl : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        public AppUserServiceImpl(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }
        public async Task<List<AppUser>> All()
        {
            return await _appUserRepository.All();
        }

        public async Task<(bool, string, string)> Login(string username, string password)
        {
            return await _appUserRepository.Login(username, password);
        }

        public async Task<(bool, string)> Register(AppUser appUser)
        {
            return await _appUserRepository.Register(appUser);
        }
    }
}
