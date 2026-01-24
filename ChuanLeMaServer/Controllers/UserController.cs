using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Repository.Interfaces;
using Service.Interfaces;

namespace ChuanLeMaServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private IAppUserRepository _appuserrespo;
        private readonly IAppUserService _appuserservice;
        public UserController(
            //   IAppUserRepository appuserrespo,
            IAppUserService appuserservice
            )
        {
            _appuserservice = appuserservice;
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var user = await _appuserservice.All();
            return Ok(user);
        }
    }
}
