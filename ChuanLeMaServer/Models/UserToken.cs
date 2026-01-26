using System.Security.Claims;

namespace ChuanLeMaServer.Models
{
    /// <summary>
    /// 用户令牌实体 用于缓存
    /// </summary>
    public class UserToken
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}
