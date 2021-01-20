using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.Tools
{
    /// <summary>
    /// 加解密工具
    /// </summary>
    public static class EncryptUtil
    {
        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Md5(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Format("eday{0}int{1}", new Random().Next(123456789, 987654321), DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            var result = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(value))).ToLower().Replace("-", "");
            return result;
        }

        /// <summary>
        /// 旧的加密
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>解密字符串</returns>
        public static string TransEcrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return "";
            }
            try
            {
                //return new MFunctionClass().TransEnC(plaintext).ToString();
                return plaintext;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 旧的解密
        /// </summary>
        /// <param name="encryptString">解密字符串</param>
        /// <returns>明文</returns>
        public static string TransDecrypt(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }
            try
            {
                //return new MFunctionClass().TransDeC(encryptString).ToString();
                return encryptString;
            }
            catch
            {
                return "";
            }
        }
    }
}
