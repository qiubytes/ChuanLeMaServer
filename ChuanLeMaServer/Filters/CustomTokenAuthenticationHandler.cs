using ChuanLeMaServer.Models;
using Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ChuanLeMaServer.Filters
{
    /* 
        1. Authentication（认证） - 解决"你是谁"的问题
        csharp
        // 对应：app.UseAuthentication()
        // 核心：CustomTokenAuthenticationHandler
        // 职责：
        // 1. 验证Token是否有效
        // 2. 创建ClaimsPrincipal（包含用户身份、角色、权限等Claims）
        // 3. 返回AuthenticationTicket
      
        实际上是给HttpContext.User赋值ClaimsPrincipal
     */
    /// <summary>
    /// Token 认证处理程序
    /// </summary>
    public class CustomTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IMemoryCache _memoryCache;

        public CustomTokenAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMemoryCache memoryCache)
            : base(options, logger, encoder, clock)
        {
            _memoryCache = memoryCache;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                // 1. 获取请求头中的 Token
                if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    // 没有Authorization头，返回NoResult（不是失败）
                    return AuthenticateResult.NoResult();
                }

                // 2. 提取实际 Token
                var actualToken = authHeader[0]?.ToString();

                if (string.IsNullOrEmpty(actualToken))
                {
                    return AuthenticateResult.NoResult();
                }

                // 3. 从缓存验证 Token
                UserToken utoken = _memoryCache.Get<UserToken>(actualToken);
                if (utoken == null)
                {
                    return AuthenticateResult.Fail("Token失效");
                }

                // 4. 创建 ClaimsPrincipal
                // 注意：这里使用 Scheme.Name 作为认证类型
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    utoken.Claims,
                    Scheme.Name,  // 使用认证方案名称
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // 5. 保存 UserName 到 Items（可选）
                Context.Items["UserName"] = utoken.UserName;

                // 6. 返回认证成功的票据
                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                // 记录异常
                Logger.LogError(ex, "Token认证过程中发生错误");
                return AuthenticateResult.Fail("认证过程中发生错误");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // 自定义 401 响应（保持你原有的响应格式）
            Response.StatusCode = 401;
            Response.ContentType = "application/json";
            var result = new ResponseResult<string>(msg: "未授权，请提供有效的Token", code: -1);
            return Response.WriteAsJsonAsync(result);
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // 自定义 403 响应
            Response.StatusCode = 403;
            Response.ContentType = "application/json";
            var result = new ResponseResult<string>(msg: "禁止访问，权限不足", code: -1);
            return Response.WriteAsJsonAsync(result);
        }
    }
}
