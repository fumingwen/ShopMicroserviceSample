using System.Text;

namespace Common.Tools
{
    /// <summary>
    /// 文件操作工具类
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// 从Json File文件读取并生成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T FromJsonFile<T>(string path, Encoding encoding=null) where T : class
        {
            try
            {
                if (encoding == null) encoding = System.Text.Encoding.Default;
                var jsons = System.IO.File.ReadAllLines(path, encoding);
                var json = string.Join("", jsons);
                return JsonUtil.FromJson<T>(json);
            }
            catch
            {
                return default;
            }
        }
    }
}
