using ChuanLeMaServer.Models;
using Common;
using Dto;
using Dto.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace ChuanLeMaServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private IAppUserRepository _appuserrespo;
        private readonly IAppUserService _appuserservice;
        /// <summary>
        /// 内存缓存服务
        /// </summary>
        private readonly IMemoryCache _memoryCache;
        public UserController(
            //   IAppUserRepository appuserrespo,
            IAppUserService appuserservice,
            IMemoryCache memoryCache
            )
        {
            _appuserservice = appuserservice;
            _memoryCache = memoryCache;
        }
        [HttpGet("test")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Test()
        {
            var user = await _appuserservice.All();
            return Ok(user);
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseResult<string>>> Login([FromBody] Dto.User.LoginRequestDto loginRequest)
        {
            var (success, message, token) = await _appuserservice.Login(loginRequest.UserName, CryptoUtils.DoubleMD5(loginRequest.Password));
            if (success)
            {
                UserToken userToken = new UserToken
                {
                    UserName = loginRequest.UserName,
                    Token = token,
                    Claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, loginRequest.UserName),
                        new System.Security.Claims.Claim(ClaimTypes.Role,"Admin"),
                        new System.Security.Claims.Claim(ClaimTypes.Role,"SuperAdmin"),
                    }

                };
                _memoryCache.Set(token, userToken, TimeSpan.FromDays(2)); // 设置缓存，过期时间为2小时
                return Ok(new ResponseResult<string>(token));
            }
            else
            {
                return Unauthorized(new ResponseResult<string>(code: -1, msg: message));
            }
        }
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseResult<string>>> Register([FromBody] Dto.User.RegisterRequestDto registerRequest)
        {
            var appUser = new Model.AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerRequest.UserName,
                Password = CryptoUtils.DoubleMD5(registerRequest.Password)
            };
            var (success, message) = await _appuserservice.Register(appUser);
            if (success)
            {
                return Ok(new ResponseResult<string>("注册成功"));
            }
            else
            {
                return BadRequest(new ResponseResult<string>(code: -1, msg: message));
            }
        }

    }
}
