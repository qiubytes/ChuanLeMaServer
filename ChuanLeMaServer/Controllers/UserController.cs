using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Repository.Interfaces;

namespace ChuanLeMaServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IAppUserRepository _appuserrespo;
        public UserController(IAppUserRepository appuserrespo)
        {
            _appuserrespo = appuserrespo;
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var user = await _appuserrespo.All();
            return Ok(user);
        }
    }
}
