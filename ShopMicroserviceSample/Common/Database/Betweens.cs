using System;
using System.Collections.Generic;

namespace Common.Database
{
    /// <summary>
    /// 条件范围
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class Betweens<T>
    {
        /// <summary>
        /// 开始
        /// </summary>
        public T From { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public T End { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Betweens()
        { 
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        public Betweens(T from, T end)
        {
            this.From = from;
            this.End = end;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Betweens<T> Between(T from, T end)
        {
            return new Betweens<T>(from, end);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var type = typeof(T).Name;
            var types = new List<string>() { "Int32", "Int64", "Decimal", "DateTime" };

            if (type == "DateTime")
            {
                return string.Format("'{0}' and '{1}'", ((DateTime)(object)this.From).ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)(object)this.End).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (types.Contains(type))
            {
                return string.Format("{0} and {1}", this.From, this.End);
            }
            else
            {
                throw new Exception("不支持的类型");
            }
        }
    }
}
