/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    public static class SteamHelper
    {
        #region 扩展方法
        public static string ReadToEnd(this Stream stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
            {
                return reader.ReadToEndAsync().Result;
            }
        }

        public static Stream ToStream(this string content)
        {
            MemoryStream stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.Write(content);
            sw.Flush();

            stream.Position = 0;
            return stream;
        }
        #endregion 

        public static string GetMd5(this Stream inputStream)
        {
            try
            {
                if (inputStream.Position != 0)
                {
                    if (inputStream.CanSeek)
                    {
                        inputStream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        throw new Exception("计算md5必须从0开始");
                    }
                }
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(inputStream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("计算Md5失败，错误:" + ex.Message);
            }
        }
    }
}
