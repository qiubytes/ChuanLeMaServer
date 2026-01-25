using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class CryptoUtils
    {
        /// <summary>
        /// 两次MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DoubleMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                // 第一次MD5
                byte[] firstHash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                // 第二次MD5
                byte[] secondHash = md5.ComputeHash(firstHash);

                // 转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < secondHash.Length; i++)
                {
                    sb.Append(secondHash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
