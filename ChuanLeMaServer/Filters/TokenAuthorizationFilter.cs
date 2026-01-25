using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ChuanLeMaServer.Filters
{
    /// <summary>
    /// token身份验证过滤器
    /// // 在 Controller 或 Action 上使用
    ///[TypeFilter(typeof(TokenAuthorizationFilter))]
    /// 
    /// </summary>
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IMemoryCache _memoryCache;

        public TokenAuthorizationFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 1. 检查是否允许匿名访问
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
                return;

            // 2. 获取请求头中的 Token
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                context.Result = new UnauthorizedObjectResult(new ResponseResult<string>(msg: "缺少Authorization头", code: -1));
                return;
            }

            // 3. 验证 Token 格式 (Bearer token)
            //var token = authHeader.ToString();
            //if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            //{
            //    context.Result = new UnauthorizedObjectResult(new { Message = "Token 格式错误，应为 Bearer token" });
            //    return;
            //}

            // 4. 提取实际 Token
            var actualToken = authHeader[0];// token.Substring("Bearer ".Length).Trim();
             
            var userName = _memoryCache.Get<string>(actualToken);
            if (string.IsNullOrEmpty(userName))
            {
                context.Result = new UnauthorizedObjectResult(new ResponseResult<string>(msg: "token失效", code: -1));
                return;
            }
            /*
             // HttpContext.Items 是一个字典，存在于单个请求的生命周期内
                public abstract class HttpContext
                {
                    public abstract IDictionary<object, object?> Items { get; }
                }

                // 它的特点是：
                // 1. 作用域：仅限于当前 HTTP 请求
                // 2. 存储位置：服务器内存中
                // 3. 生命周期：请求开始时创建，请求结束后销毁
                // 4. 访问权限：仅服务器端代码可以访问 
             */
            context.HttpContext.Items["UserName"] = userName;
        }

    }


}
