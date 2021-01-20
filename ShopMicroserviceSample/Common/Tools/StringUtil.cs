using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] Split(this string source, string split)
        {
            return source.Split(new string[] { split });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] Split(this string source, params string[] split)
        {
            return source.Split(split, StringSplitOptions.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Contains(this string source, params string[] param)
        {
            foreach (var s in param)
            {
                if (!string.IsNullOrEmpty(s) && source.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 字符串是否存在指定的字符串列表里面
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="param">字符串列表</param>
        /// <returns></returns>
        public static bool ExistsIn(this string source, params string[] param)
        {
            foreach (var s in param)
            {
                if (!string.IsNullOrEmpty(s) && source.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Contains(this string source, List<string> param)
        {
            foreach (var s in param)
            {
                if (!string.IsNullOrEmpty(s) && source.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Includes(this int source, params int[] param)
        {
            foreach (var i in param)
            {
                if (source == i) return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool Includes(this int source, List<int> param)
        {
            foreach (var i in param)
            {
                if (source == i) return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatValue(string type, object value)
        {
            if (value == null) return "null";

            if (Constants.NullValues.Contains(value.ToString())) return "null";

            if (type == "String") return string.Format("'{0}'", value);

            if (type == "DateTime") return string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff"));

            return value.ToString();
        }

        /// <summary>
        /// 强制把Xiaoyu的用户工号转为整数
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static long FromXiaoyu(this string userNo)
        {
            var success = long.TryParse(userNo, out long result);
            return success ? result : Constants.Xiaoyu;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static StringBuilder AppendFormatNewLine(this StringBuilder sb, string format,  object arg)
        {
            return sb.AppendFormat(format + System.Environment.NewLine, arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendFormatNewLine(this StringBuilder sb, string format, params object[] args)
        {
            return sb.AppendFormat(format + System.Environment.NewLine, args);
        }

        /// <summary>
        /// List转换为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="quotation"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinToString<T>(this List<T> list, string quotation = null, string separator = ",")
        {
            var join = string.Join(separator, list.ToArray());
            if (!string.IsNullOrEmpty(quotation))
            {
                join = join.Replace(separator, string.Format("{0}{1}{0}", quotation, separator));
                join = string.Format("{0}{1}{0}", quotation, join);
            }
            return join;
        }

        /// <summary>
        /// 获得全局唯一标识
        /// </summary>
        /// <returns></returns>
        public static string Guid()
        {
            return System.Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

        /// <summary>
        /// 清除空格
        /// </summary>
        /// <returns></returns>
        public static string TrimAll(this string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Trim().Replace(" ", "").Replace(" ", "");
        }
    }
}
