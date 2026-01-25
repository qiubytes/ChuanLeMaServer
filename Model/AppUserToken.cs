using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// 用户登录令牌实体类
    /// </summary>
    public class AppUserToken
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required string Token { get; set; }
        public required DateTime AddTime { get; set; }
    }
}
