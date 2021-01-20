using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Linq;
using Common.Tools;
using System.Linq.Expressions;
using Common.Helper;

namespace Common.Database
{
    /// <summary>
    /// 通用不分页搜索请求
    /// </summary>
    /// <typeparam name="T">搜索参数</typeparam>
    public class SearchRequest<T> where T : new()
    {
        /// <summary>
        /// 搜索参数
        /// </summary>
        public T Model { get; set; }

        /// <summary>
        /// 每页记录数，-1全选
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; }

        private string autoWhere = "";

        private string manualWhere = "";

        /// <summary>
        /// 不分页搜索请求
        /// </summary>
        public SearchRequest()
        {
            this.PageSize = 10;
        }

        /// <summary>
        /// 不分页搜索请求
        /// </summary>
        /// <param name="model">搜索参数</param>
        public SearchRequest(T model)
        {
            this.Model = model;

            this.PageSize = 10;
        }

        /// <summary>
        /// 不分页搜索请求
        /// </summary>
        /// <param name="model">搜索参数</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pageSize">每页记录数</param>
        public SearchRequest(T model, string orderBy, int pageSize)
        {
            this.Model = model;
            this.OrderBy = orderBy;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 设置手动条件
        /// </summary>
        /// <param name="manualWhere"></param>
        public void ManualWhere(String manualWhere)
        {
            this.manualWhere = manualWhere;
        }

        /// <summary>
        /// 设置手动条件
        /// </summary>
        /// <param name="expression"></param>
        public void ManualWhere(Expression<Func<T, bool>> expression)
        {
            this.manualWhere = ExpressHelper.SqlWhere(expression);
        }

        /// <summary>
        /// 设置手动条件
        /// </summary>
        /// <param name="expression"></param>
        public void ManualWhere<T1>(Expression<Func<T,T1, bool>> expression)
        {
            this.manualWhere = ExpressHelper.SqlWhere(expression);
        }

        /// <summary>
        /// 设置手动条件
        /// </summary>
        /// <param name="expression"></param>
        public void ManualWhere<T1,T2>(Expression<Func<T, T1,T2, bool>> expression)
        {
            this.manualWhere = ExpressHelper.SqlWhere(expression);
        }
        /// <summary>
        /// 设置手动条件
        /// </summary>
        /// <param name="expression"></param>
        public void ManualWhere<T1, T2, T3>(Expression<Func<T, T1, T2,T3, bool>> expression)
        {
            this.manualWhere = ExpressHelper.SqlWhere(expression);
        }

        /// <summary>
        /// 手动条件组装
        /// </summary>
        /// <returns></returns>
        public String ManualWhere()
        {
            return this.manualWhere;
        }

        /// <summary>
        /// 自动条件组装
        /// </summary>
        /// <returns></returns>
        public string AutoWhere()
        {
            if (!string.IsNullOrEmpty(autoWhere)) return autoWhere;

            var model = this.Model;

            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo p in model.GetType().GetProperties())
            {
                object value = null;
                try
                {
                    value = p.GetValue(model, null);
                }
                catch (Exception)
                {

                }
                if (value == null)
                {
                    continue;
                }

                var tableName = "";
                string columnName = null;
                var filedComment =(FiledComment) p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment != null)
                {
                    if (!filedComment.AutoWhere) continue;
                    tableName = filedComment.TableName;
                    if (!string.IsNullOrEmpty(filedComment.ColumnName))
                    {
                        columnName = filedComment.ColumnName;
                    }
                }

                tableName = this.TableName(tableName);
                var name = p.Name;
                string typeName=p.RealTypeName();

                switch (typeName)
                {
                    case "String":
                        if (value is IList list1)
                        {
                            var list = list1.Cast<object>().Select(o => o.ToString()).ToList();

                            if (name.StartsWith("NotIn"))
                            {
                                sb.AppendFormat(" and {0}[{1}] not in ('{2}')", tableName, OriginNameFromList(name, true, columnName), String.Join(",", list.ToArray()).Replace(",", "','"));
                            }
                            else
                            {
                                sb.AppendFormat(" and {0}[{1}] in ('{2}')", tableName, OriginNameFromList(name, false, columnName), String.Join(",", list.ToArray()).Replace(",", "','"));
                            }
                        }
                        else
                        {
                            if (name.StartsWith("NotEqual"))
                            {
                                name = name.Substring(8);
                                if (Constants.NullValues.Contains(value.ToString()))
                                {
                                    sb.AppendFormat(" and {0}[{1}] is not null", tableName, OriginName(name, columnName));
                                }
                                else
                                {
                                    sb.AppendFormat(" and {0}[{1}] <> '{2}'", tableName, OriginName(name, columnName), value);
                                }
                            }
                            else if (name.StartsWith("NotLike"))
                            {
                                sb.AppendFormat(" and {0}[{1}] not like '%{2}%'", tableName, OriginName(name.Substring(7), columnName), value);
                            }
                            else if (name.StartsWith("Between") && !name.StartsWith("Betweens"))
                            {
                                sb.AppendFormat(" and {0}[{1}] between {2}", tableName, OriginName(name.Substring(7), columnName), value);
                            }
                            else if (name.StartsWith("NotBetween"))
                            {
                                sb.AppendFormat(" and {0}[{1}] not between {2}", tableName, OriginName(name.Substring(10), columnName), value);
                            }
                            else
                            {
                                if (Constants.NullValues.Contains(value.ToString()))
                                {
                                    sb.AppendFormat(" and {0}[{1}] is null", tableName, OriginName(name, columnName));
                                }
                                else if (filedComment != null && filedComment.StringEqual)
                                {
                                    sb.AppendFormat(" and {0}[{1}]='{2}'", tableName, OriginName(name, columnName), value);
                                }
                                else
                                {
                                    sb.AppendFormat(" and {0}[{1}] like '%{2}%'", tableName, OriginName(name, columnName), value);
                                }
                            }
                        }
                        break;
                    case "Int32":
                    case "Int64":
                    case "Decimal":
                    case "DateTime":
                        if (value is IList list2)
                        {
                            var list = list2.Cast<object>().Select(o => o.ToString()).ToList();

                            if (name.StartsWith("NotIn"))
                            {
                                sb.AppendFormat(" and {0}[{1}] not in ({2})", tableName, OriginNameFromList(name, true, columnName), String.Join(",", list.ToArray()));
                            }
                            else
                            {
                                sb.AppendFormat(" and {0}[{1}] in ({2})", tableName, OriginNameFromList(name, false, columnName), String.Join(",", list.ToArray()));
                            }
                        }
                        else if (name.StartsWith("NotEqual"))
                        {
                            name = name.Substring(8);
                            if (Constants.NullValues.Contains(value.ToString()))
                            {
                                sb.AppendFormat(" and {0}[{1}] is not null", tableName, OriginName(name, columnName));
                            }
                            else
                            {
                                sb.AppendFormat(" and {0}[{1}] <> {2}", tableName, OriginName(name, columnName), FormatValue(typeName, value));
                            }
                        }
                        else if (name.StartsWith("Betweens"))
                        {
                            sb.AppendFormat(" and {0}[{1}] between {2}", tableName, OriginName(name.Substring(8), columnName), value);
                        }
                        else if (name.StartsWith("Min"))
                        {
                            sb.AppendFormat(" and {0}[{1}]>={2}", tableName, OriginName(name.Substring(3), columnName), FormatValue(typeName, value));
                        }
                        else if (name.StartsWith("Max"))
                        {
                            sb.AppendFormat(" and {0}[{1}]<={2}", tableName, OriginName(name.Substring(3), columnName), FormatValue(typeName, value));
                        }
                        else
                        {
                            if (Constants.NullValues.Contains(value.ToString()))
                            {
                                sb.AppendFormat(" and {0}[{1}] is null", tableName, OriginName(name, columnName));
                            }
                            else
                            {
                                sb.AppendFormat(" and {0}[{1}]={2}", tableName, OriginName(name, columnName), FormatValue(typeName, value));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            autoWhere = sb.ToString();
            return autoWhere;
        }

        private string TableName(string tableName)
        {
            return string.IsNullOrEmpty(tableName) ? "" : string.Format("{0}.", tableName);
        }

        private string FormatValue(string typeName, object value)
        {
            return typeName == "DateTime" ? string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff")) : string.Format("{0}", value);
        }

        private string OriginName(string name, string columnName = null)
        {
            return string.IsNullOrEmpty(columnName) ? name : columnName;
        }

        private string OriginNameFromList(string name, bool notIn = false, string columnName = null)
        {
            if (!string.IsNullOrEmpty(columnName)) return columnName;
            if (notIn) name = name.Substring(5);
            if (name.EndsWith("ies")) return string.Format("{0}{1}", name.Substring(0, name.Length - 3), "y");
            if (name.EndsWith("ses")) return name.Substring(0, name.Length - 2);
            if (name.EndsWith("thes")) return name.Substring(0, name.Length - 2);
            if (name.EndsWith("s")) return name.Substring(0, name.Length - 1);
            return name;
        }
    }
}
