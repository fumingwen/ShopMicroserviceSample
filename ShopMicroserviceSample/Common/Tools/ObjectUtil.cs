using Common.Database;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.Tools
{
    /// <summary>
    /// 对象操作工具
    /// </summary>
    public static class ObjectUtil
    {

        /// <summary>
        /// 获取某个对象的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<T>(T entity, string propertyName)
        {
            Type type = typeof(T);
            if (type == null)
            {
                return null;
            }
            return type.GetProperty(propertyName);
        }

        /// <summary>
        /// 获取某个对象属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T entity, string propertyName)
        {
            var property = GetProperty(entity, propertyName);
            if (property == null) return null;
            return property.GetValue(entity);
        }

        /// <summary>
        /// 对象复制
        /// </summary>
        /// <param name="source">对象来源</param>
        /// <param name="target">对象目标</param>
        /// <param name="ignoreNull">是否忽略null值，默认忽略</param>
        /// <param name="ignoreProperties">忽略哪些属性不复制</param>
        /// <param name="includeNullProperties">空值也要带上</param>
        public static void Copy(object source, object target, bool ignoreNull = true, List<string> ignoreProperties = null, List<string> includeNullProperties = null)
        {
            foreach (PropertyInfo p in source.GetType().GetProperties())
            {
                if (ignoreProperties != null && ignoreProperties.Contains(p.Name))
                {
                    continue;
                }
                var value = p.GetValue(source, null);
                if (!ignoreNull || value != null || (value == null && includeNullProperties != null && includeNullProperties.Contains(p.Name)))
                {
                    p.SetValue(target, value, null);
                }
            }
        }

        /// <summary>
        /// 获取属性的实际类型
        /// </summary>
        /// <param name="propertyInfo">属性</param>
        /// <returns></returns>
        public static string RealTypeName(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType.GenericTypeArguments;
            if (type != null && type.Length > 0)
            {
                return propertyInfo.PropertyType.GenericTypeArguments[0].Name;
            }
            else
            {
                return propertyInfo.PropertyType.Name;
            }
        }

        /// <summary>
        /// 将对象转换为key1=value1&key2=value2的形式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据</param>
        /// <param name="excludeNull">排除null的值</param>
        /// <param name="dateTimeFormat">时间转换格式</param>
        /// <returns></returns>
        public static string ToFormUrlEncoded<T>(T data, bool excludeNull = true, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var sb = new StringBuilder();

            foreach (PropertyInfo p in data.GetType().GetProperties())
            {
                object value = null;
                try
                {
                    value = p.GetValue(data, null);
                }
                catch { }

                if (excludeNull && value == null) continue;

                string typeName = p.RealTypeName();
                if (typeName != "DateTime")
                {
                    sb.AppendFormat("{0}={1}&", p.Name, value.ToString());
                }
                else
                {
                    sb.AppendFormat("{0}={1}&", p.Name, ((DateTime)value).ToString(dateTimeFormat));
                }
            }

            return sb.Append("E-N-D").Replace("&E-N-D", "").Replace("E-N-D", "").ToString();
        }

        /// <summary>
        /// 从实体类获得表格名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ignoreComment">ignoreComment</param>
        /// <returns></returns>
        public static string GetTableName<T>(bool ignoreComment = true)
        {
            var type = typeof(T);
            if (!ignoreComment)
            {
                var filedComment = (FiledComment)type.GetCustomAttribute(typeof(FiledComment));
                if (filedComment != null)
                {
                    var name = filedComment.TableName;
                    if (!string.IsNullOrEmpty(name)) return name;
                }
            }
            return type.Name;
        }

        /// <summary>
        /// 通过实体获得对应表的字段名称
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <returns></returns>
        public static string GetTableColumns<T>()
        {
            var type = typeof(T);
            var sb = new StringBuilder();

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.Name!=null && p.Name == "PrimaryKey") continue;

                var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment == null)
                {
                    sb.AppendFormat("{0}, ", p.Name);
                }
                else
                {
                    if (filedComment.Ignore) continue;
                    var tableName = filedComment.TableName;
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        tableName = $"{tableName}.";
                    }
                    var columnName = p.Name;
                    if (!string.IsNullOrEmpty(filedComment.ColumnName)) columnName = $"{ filedComment.ColumnName} as {p.Name}";
                    sb.AppendFormat("{0}{1}, ", tableName, columnName);
                }
            }
            sb.Append("ENDEND").Replace(", ENDEND", "").Replace("ENDEND", "");
            return sb.ToString();
        }
    }
}
