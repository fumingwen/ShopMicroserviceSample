using Common.Caches;
using Common.Database;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DataBasic
{
    public class DataTool
    {
        /// <summary>
        /// 通过实体获得对应表的字段名称
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="reload">强制重载</param>
        /// <param name="ignoreColumns">忽略的字段</param>
        /// <returns></returns>
        public static string SelectColumns<T>(ICache cache=null, bool reload=false, List<string> ignoreColumns=null)
        {
            var type = typeof(T);
            string typeKey = $"{type.FullName}.Select";
            string columns;

            if (cache != null && !reload)
            {
                columns = cache.Get<string>(typeKey);
                if (!string.IsNullOrEmpty(columns)) return columns;
            }

            var sb = new StringBuilder();

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (ignoreColumns != null && ignoreColumns.Contains(p.Name)) continue;

                var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment == null)
                {
                    sb.AppendFormat("[{0}], ", p.Name);
                }
                else
                {
                    if (filedComment.Ignore) continue;
                    var tableName = filedComment.TableName;
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        tableName = $"[{tableName}].";
                    }
                    var columnName = p.Name;
                    if (!string.IsNullOrEmpty(filedComment.ColumnName)) columnName = $"[{ filedComment.ColumnName}] as {p.Name}";
                    sb.AppendFormat("{0}{1}, ", tableName, columnName);
                }
            }

            columns=sb.ToString().TrimEnd(' ', ',');
            if (cache != null)
            {
                cache.Add(typeKey, columns, 86400);
            }
            return columns;
        }

        /// <summary>
        /// 通过实体获得对应表的字段名称
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="reload">强制重载</param>
        /// <param name="cache">缓存</param>
        /// <param name="ignoreColumns">忽略的字段</param>
        /// <returns></returns>
        public static string InsertColumns<T>(ICache cache = null, bool reload = false, List<string> ignoreColumns = null)
        {
            var type = typeof(T);
            string typeKey = $"{type.FullName}.Insert";
            string columns;

            if (cache != null && !reload)
            {
                columns = cache.Get<string>(typeKey);
                if (!string.IsNullOrEmpty(columns)) return columns;
            }

            var sb = new StringBuilder();

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (ignoreColumns != null && ignoreColumns.Contains(p.Name)) continue;

                var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment == null)
                {
                    sb.AppendFormat("[{0}], ", p.Name);
                }
                else
                {
                    if (filedComment.Ignore || filedComment.IsIdentity || filedComment.InsertIgnore) continue;
                    var columnName = p.Name;
                    if (!string.IsNullOrEmpty(filedComment.ColumnName)) columnName = $"[{ filedComment.ColumnName}]";
                    sb.AppendFormat("{0}, ", columnName);
                }
            }

            columns = sb.ToString().TrimEnd(' ', ',');
            if (cache != null)
            {
                cache.Add(typeKey, columns, 86400);
            }
            return columns;
        }

        /// <summary>
        /// 通过实体获得对应表的字段名称
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="reload">强制重载</param>
        /// <param name="cache">缓存</param>
        /// <param name="ignoreColumns">忽略的字段</param>
        /// <returns></returns>
        public static string UpdateColumns<T>(ICache cache = null, bool reload = false, List<string> ignoreColumns = null)
        {
            var type = typeof(T);
            string typeKey = $"{type.FullName}.Update";
            string columns;

            if (cache != null && !reload)
            {
                columns = cache.Get<string>(typeKey);
                if (!string.IsNullOrEmpty(columns)) return columns;
            }

            var sb = new StringBuilder();

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (ignoreColumns != null && ignoreColumns.Contains(p.Name)) continue;

                var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment == null)
                {
                    sb.AppendFormat("[{0}] = @{1}, ", p.Name, p.Name);
                }
                else
                {
                    if (filedComment.Ignore || filedComment.IsIdentity || filedComment.UpdateIgnore) continue;
                    var columnName = p.Name;
                    if (!string.IsNullOrEmpty(filedComment.ColumnName)) columnName = $"[{ filedComment.ColumnName}]";
                    sb.AppendFormat("{0} = @{1}, ", columnName, p.Name);
                }
            }

            columns = sb.ToString().TrimEnd(' ',',');
            if (cache != null)
            {
                cache.Add(typeKey, columns, 86400);
            }
            return columns;
        }
    }
}
