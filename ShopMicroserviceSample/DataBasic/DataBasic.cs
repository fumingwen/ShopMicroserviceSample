using Dapper;
using Common;
using Common.Caches;
using Common.Database;
using Common.Helper;
using Common.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace DataBasic
{
    /// <summary>
    /// 数据库基本操作
    /// </summary>
    /// <typeparam name="Te">实体对象</typeparam>
    /// <typeparam name="Tk">主键</typeparam>
    /// <typeparam name="Tm">搜索参数</typeparam>
    public abstract class DataBasic<Te, Tk, Tm> : IDataBasic<Te, Tk, Tm> where Te : EntityBase<Tk>, new() where Tm : new()
    {
        protected readonly IDbContext db;

        public TableMapper<Te> TableMapper { get; set; }

        public DataBasic(IConfiguration configuration)
        {
            db = new DbFactory(configuration).db;
        }
        public DataBasic(string configKey)
        {
            db = new DbFactory(configKey).db;
        }

        public DataBasic(string configKey, string defaultKey)
        {
            db = new DbFactory(configKey, defaultKey).db;
        }
        
        /// <summary>
        /// 缓存
        /// </summary>
        protected ICache Cache { get; set; }

        #region 通用执行和查询功能

        protected int Execute(string sql, object param = null)
        {
            var connection = db.CreateConnection();
            int count = connection.Execute(sql, param);
            db.CloseConnection(connection);
            return count;
        }

        protected T ExecuteScalar<T>(string sql, object param = null)
        {
            var connection = db.CreateConnection();
            T count = connection.ExecuteScalar<T>(sql, param);
            db.CloseConnection(connection);
            return count;
        }

        protected bool Truncate()
        {
            var sql = $"truncate table {TableMapper.TableName}";
            this.Execute(sql);
            return true;
        }

        public T Query<T>(string sql, object param = null)
        {
            var connection = db.CreateConnection();
            T result = connection.Query<T>(sql, param).FirstOrDefault();
            db.CloseConnection(connection);
            return result;
        }

        public List<T> Querys<T>(string sql, object param = null)
        {
            var connection = db.CreateConnection();
            List<T> result = connection.Query<T>(sql, param).ToList();
            db.CloseConnection(connection);
            return result;
        }

        #endregion

        #region 具体增删改查

        public Tk Insert(Te entity)
        {
            var sql = string.Format("insert into [{0}] ({1})", TableMapper.TableName, TableMapper.InsertFileds);
            if (string.IsNullOrEmpty(TableMapper.InsertWhere))
            {
                sql += string.Format(" values({0})", TableMapper.InsertValues);
            }
            else
            {
                sql += string.Format(" select {0} {1}", TableMapper.InsertValues, TableMapper.InsertWhere);
            }

            if (this.TableMapper.InsertReturnIdentity)
            {
                sql += "; select @@identity as primaryKey; ";
                var primaryKey = this.ExecuteScalar<Tk>(sql, entity);
                entity.SetPrimaryKey(primaryKey);
                return primaryKey;
            }
            else
            {
                var primaryKey = this.Execute(sql, entity);
                var type = typeof(Tk).Name;
                if (type == "Int64") return (Tk)(object)long.Parse(primaryKey.ToString());
                return (Tk)(object) primaryKey;
            }
        }

        public int BatchInsert(List<Te> entities, bool truncate = false)
        {
            if (entities == null || entities.Count < 1) return 0;

            if (truncate && TableMapper.TableName.ToLower().EndsWith("temp"))
            {
                this.Truncate();
            }

            var sb = new StringBuilder();
            sb.AppendFormat("insert into [{0}] ({1}) select {2} from(", TableMapper.TableName, TableMapper.InsertFileds, TableMapper.InsertValues.Replace("@", ""));

            var properties = typeof(Te).GetProperties();
            foreach (Te item in entities)
            {
                sb.Append("select ");
                foreach (var p in properties)
                {
                    var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                    if (filedComment != null && filedComment.Ignore) continue;
                    var @object = p.GetValue(item, null);
                    if (@object != null && @object is IList) continue;
                    var value = StringUtil.FormatValue(p.RealTypeName(), @object);
                    sb.AppendFormat("{0} as {1}, ", value, p.Name);
                }
                sb.Append("union all ");

            }
            sb.Append(")t").Replace(", union all "," union all ").Replace(" union all )t",")t");

            if (!string.IsNullOrEmpty(TableMapper.InsertWhere))
            {
                sb.AppendFormat(TableMapper.InsertWhere.Replace("@", "t."));
            }

            return this.Execute(sb.ToString());
        }

        protected bool SqlBulkCopy(List<Te> entities, bool truncate = false)
        {
            if (entities == null || entities.Count < 1) return true;

            var tableName = ObjectUtil.GetTableName<Te>(false);

            if (truncate && tableName.ToLower().EndsWith("temp"))
            {
                this.Truncate();
            }

            var dataTable = ListToDataTable(entities, tableName);

            var connection = db.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            SqlTransaction sqlbulkTransaction = (SqlTransaction)connection.BeginTransaction();

            SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.CheckConstraints, sqlbulkTransaction)
            {
                DestinationTableName = tableName
            };

            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }
            try
            {
                bulkCopy.WriteToServer(dataTable);
                sqlbulkTransaction.Commit();
            }
            catch
            {
                sqlbulkTransaction.Rollback();
            }
            finally
            {
                bulkCopy.Close();
                db.CloseConnection(connection);
            }

            return true;
        }

        /// <summary>
        /// 将List转换为DataTable
        /// </summary>
        /// <param name="entities">请求数据</param>
        /// <returns></returns>
        private static DataTable ListToDataTable<T>(List<T> entities, string tableName = null)
        {
            if (tableName == null) tableName = "tableName";
            DataTable dataTable = new DataTable(tableName);

            var columns = new Dictionary<string, string>();

            foreach (var p in typeof(T).GetProperties())
            {
                var propertyName = p.Name;
                var columnName = p.Name;

                var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment != null)
                {
                    if (filedComment.Ignore || filedComment.IsIdentity) continue;
                    if (!string.IsNullOrEmpty(filedComment.ColumnName))
                    {
                        columnName = filedComment.ColumnName;
                    }
                }
                columns.Add(propertyName, columnName);

                dataTable.Columns.Add(columnName);
            }

            foreach (var entity in entities)
            {
                DataRow row = dataTable.NewRow();
                foreach (var key in columns.Keys)
                {
                    int columnIndex = dataTable.Columns.IndexOf(columns[key]);
                    row[columnIndex] = entity.GetType().GetProperty(key).GetValue(entity);
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public int Delete(Tk primaryKey)
        {
            var sql = string.Format("delete [{0}] where {1}", TableMapper.TableName, string.Format(TableMapper.PrimaryKeys, primaryKey));
            return this.Execute(sql);
        }

        public int Delete(params object[] primaryKeys)
        {
            var sql = string.Format("delete [{0}] where {1}", TableMapper.TableName, string.Format(TableMapper.PrimaryKeys, primaryKeys));
            return this.Execute(sql);
        }

        public int DeleteByCondition(Tm model, Expression<Func<Te, bool>> expression)
        {
            ConditionBuilderVisitor visitor = new ConditionBuilderVisitor();
            visitor.Visit(expression);
            var sqlWhere = visitor.Condition();
            var sql = string.Format("delete [{0}] where {1}", TableMapper.TableName, sqlWhere);
            return this.Execute(sql, model);
        }
        public int DeleteByCondition(List<Tm> models, Expression<Func<Te, bool>> expression)
        {
            ConditionBuilderVisitor visitor = new ConditionBuilderVisitor();
            visitor.Visit(expression);
            var sqlWhere = visitor.Condition();
            var sql = string.Format("delete [{0}] where {1}", TableMapper.TableName, sqlWhere);
            return this.Execute(sql, models);
        }

        /// <summary>
        /// 根据表达树批量更新数据   
        /// 使用前先确保实体类的主键被加上特性 [Key]
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int UpdateByCondition(List<Te> entities, Expression<Func<Te, dynamic>> expression)
        {
            ConditionBuilderVisitor visitor = new ConditionBuilderVisitor();
            visitor.Visit(expression);
            var paramList = visitor.Condition().Trim().Split(" ").ToList();

            Type type = typeof(Te);

            var sb = new StringBuilder();

            foreach (var m in entities)
            {
                var set = string.Join(",", paramList.Select(c => $"{m.GetType().GetProperty(c).Name} = @{m.GetType().GetProperty(c).Name}"));
                var where = $"{type.GetPropertiesKey().Name}=@{type.GetPropertiesKey().Name}";
                sb.AppendFormat("update [{0}] set {1} where {2};", type.GetMappingName(), set, where);
            }

            return this.Execute(sb.ToString(),entities);
        }

        public List<Te> FindByCondition(Tm model, Expression<Func<Te, bool>> expression)
        {
            ConditionBuilderVisitor visitor = new ConditionBuilderVisitor();
            visitor.Visit(expression);
            var sqlWhere = visitor.Condition();
            Type type = typeof(Te);

            var sql = $" select * from {type.Name} where {sqlWhere}";

            return this.Querys<Te>(sql, model);
        }

        public int Update(Te entity, Expression<Func<Te, dynamic>> expression = null)
        {
            if (expression != null) return this.BatchUpdate(new List<Te>() { entity }, expression);

            var sql = string.Format("update [{0}] set {1} where {2}", TableMapper.TableName, TableMapper.UpdateString(entity), TableMapper.UpdateWhere);
            return this.Execute(sql, entity);
        }

        public int BatchUpdate(List<Te> entities, Expression<Func<Te, dynamic>> expression = null)
        {
            if (entities == null || entities.Count < 1) return 0;

            var updateColumnNames = new List<string>();

            string body = expression.Body.ToString();
            body = new Regex(@"new <>f__[^\(\)]+\(", RegexOptions.IgnoreCase).Replace(body, "");
            if (body.Substring(0, 1) == "(") body = body.Substring(1);
            if (body.Substring(body.Length - 1, 1) == ")") body = body.Substring(0, body.Length - 1);
            var bodies = body.Replace(" ", "").Split(",");
            foreach (var s in bodies)
            {
                var temp = s.Split('=');
                if (temp.Length == 2)
                {
                    updateColumnNames.Add(temp[0]);
                }
            }

            string primaryKey = "";
            PropertyInfo primaryType = null;

            var columns = new Dictionary<string, PropertyInfo>();
            var properties = typeof(Te).GetProperties();
            foreach (var p in properties)
            {
                var columnName = p.Name;
                var filedComment = (FiledComment)p.GetCustomAttribute(typeof(FiledComment));
                if (filedComment != null)
                {
                    if (!string.IsNullOrEmpty(filedComment.ColumnName)) columnName = filedComment.ColumnName;
                    if ((filedComment.IsIdentity || filedComment.IsPrimaryKey))
                    {
                        if (string.IsNullOrEmpty(primaryKey))
                        {
                            primaryKey = columnName;
                            primaryType = p;
                        }
                        continue;
                    }
                    if (filedComment.UpdateIgnore) continue;
                    if (updateColumnNames.Count > 0 && updateColumnNames.Contains(p.Name))
                    {
                        columns.Add(columnName, p);
                    }
                }
                else
                {
                    if (updateColumnNames.Count > 0 && updateColumnNames.Contains(p.Name))
                    {
                        columns.Add(columnName, p);
                    }
                }
            }

            var sb = new StringBuilder();
            var tableName = ObjectUtil.GetTableName<Te>(false);
            sb.AppendFormat("update [{0}] set ", tableName);
            foreach (var key in columns.Keys)
            {
                sb.AppendFormat("[{0}].[{1}]=t.[{1}], ", tableName, key, key);
            }
            sb.Append("from(").Replace(", from(", " from(");

            var primaryTypeName = primaryType.Name;
            foreach (Te item in entities)
            {
                sb.AppendFormat("select {0} as [{1}], ", StringUtil.FormatValue(primaryTypeName, primaryType.GetValue(item, null)), primaryKey);
                foreach (var key in columns.Keys)
                {
                    var p = columns[key];
                    var @object = columns[key].GetValue(item, null);
                    if (@object != null && @object is IList) continue;
                    string value = StringUtil.FormatValue(p.RealTypeName(), @object);
                    sb.AppendFormat("{0} as [{1}], ", value, key);
                }
                sb.Append("union all ");
            }
            sb.AppendFormat(")t where [{0}].[{1}]=t.[{1}]", tableName, primaryKey);
            sb.Replace(", union all ", " union all ").Replace("union all )t", ")t");

            return this.Execute(sb.ToString());
        }

        public Te Find(Tk primaryKey)
        {
            var sql = string.Format("select {0} from [{1}] where {2}", TableMapper.SelectFileds, TableMapper.TableName, string.Format(TableMapper.PrimaryKeys, primaryKey));
            return this.Query<Te>(sql);
        }

        public Te Find(params object[] primaryKeys)
        {
            var sql = string.Format("select {0} from [{1}] where {2}", TableMapper.SelectFileds, TableMapper.TableName, string.Format(TableMapper.PrimaryKeys, primaryKeys));
            return this.Query<Te>(sql);
        }

        #endregion

        #region 通用不分页和分页搜索

        public List<Te> SearchRequest(SearchRequest<Tm> searchRequest)
        {
            if (searchRequest.PageSize == 0) searchRequest.PageSize = 10;
            var where = this.CombinWhere(searchRequest.AutoWhere(), searchRequest.ManualWhere());
            var sql = string.Format("select{0} {1} from [{2}] {3}{4}", searchRequest.PageSize <0 ? "" : $" top {searchRequest.PageSize}", TableMapper.SelectFileds, TableMapper.TableName, TableMapper.ShortTableName, where);
            if (!string.IsNullOrEmpty(searchRequest.OrderBy)) sql += " order by " + searchRequest.OrderBy;
            return this.Querys<Te>(sql);
        }

        public int Count(SearchRequest<Tm> searchRequest)
        {
            var where = this.CombinWhere(searchRequest.AutoWhere(), searchRequest.ManualWhere());
            var sql = string.Format("select count(0) as count from [{0}] {1}{2}", TableMapper.TableName, TableMapper.ShortTableName, where);
            return this.Query<int>(sql);
        }

        private int Count(string where)
        {
            var sql = string.Format("select count(0) as count from [{0}] {1}{2}", TableMapper.TableName, TableMapper.ShortTableName, where);
            return this.Query<int>(sql);
        }

        public PageResult<List<Te>> PageRequest(PageRequest<Tm> pageRequest)
        {
            var where = this.CombinWhere(pageRequest.AutoWhere(), pageRequest.ManualWhere());
            int totalCount = this.Count(where);
            pageRequest.CheckPage(totalCount);

            var top = pageRequest.CurrentPage * pageRequest.PageSize;
            var indexFrom = top - pageRequest.PageSize + 1;
            var indexEnd = top;
            var orderBy = String.IsNullOrEmpty(pageRequest.OrderBy) ? TableMapper.DefaultOrderBy : pageRequest.OrderBy;
            var sql = string.Format("select {0} from (select top {1} ROW_NUMBER() over( order by {2}) as RowNumber, {3} from [{4}] {5}{6}) t where RowNumber between {7} and {8}", TableMapper.SelectMappingFileds, top, orderBy, TableMapper.SelectFileds, TableMapper.TableName, TableMapper.ShortTableName, where, indexFrom, indexEnd);

            var list = this.Querys<Te>(sql);
            return new PageResult<List<Te>>(list, totalCount, pageRequest.PageSize, pageRequest.CurrentPage);
        }

        protected string CombinWhere(string autoWhere, string manualWhere = "")
        {
            var combinWhere = string.Format("{0}{1}", autoWhere, manualWhere);

            if (string.IsNullOrEmpty(combinWhere)) return "";

            if (this.TableMapper.MappingFileds != null && this.TableMapper.MappingFileds.Count > 0)
            {
                for (var i = 0; i < TableMapper.OriginalFileds.Count; i++)
                {
                    var originalFiled = TableMapper.OriginalFileds[i];
                    var mappingFiled = TableMapper.MappingFileds[i];
                    if (string.IsNullOrEmpty(mappingFiled)) mappingFiled = originalFiled;
                    if (mappingFiled != originalFiled)
                    {
                        combinWhere = combinWhere.Replace(string.Format("[{0}]", mappingFiled), string.Format("[{0}]", originalFiled));
                    }
                }
            }

            combinWhere = (" where" + combinWhere).Replace(" where and ", " where ").Replace(" where AND ", " where ").Replace(" where And ", " where ");
            return combinWhere;
        }

        protected Queryable<T, T> Queryable<T>(string shortName = null, bool mapColumns = false)
        {
            return new Queryable<T, T>(db, shortName, mapColumns);
        }

        protected Queryable<T1, T> Queryable<T1, T>(string shortName = null, bool mapColumns = false)
        {
            return new Queryable<T1, T>(db, shortName, mapColumns);
        }

        /// <summary>
        /// 通过对象获取字段
        /// </summary>
        /// <param name="dataOperateType">操作类型，筛选、更新、插入</param>
        /// <param name="reload">是否重新加载，忽略缓存</param>
        /// <param name="ignoreColumns">忽略哪些字段</param>
        /// <returns></returns>
        protected string GetColumnNames<T>(DataOperateType dataOperateType = DataOperateType.Select, bool reload = false, List<string> ignoreColumns = null)
        {
            switch (dataOperateType)
            {
                case DataOperateType.Select:
                    return DataTool.SelectColumns<T>(this.Cache, reload, ignoreColumns);
                case DataOperateType.Insert:
                    return DataTool.InsertColumns<T>(this.Cache, reload, ignoreColumns);
                case DataOperateType.Update:
                    return DataTool.UpdateColumns<T>(this.Cache, reload, ignoreColumns);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sum数据库字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        protected T Sum<T>(T source)
        {
            return default;
        }

        /// <summary>
        /// Count(0)或数据库字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        protected int Count<T>(T source)
        {
            return default;
        }

        /// <summary>
        /// Floor(0)或数据库字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        protected T Floor<T>(T source)
        {
            return default;
        }

        #endregion
    }
}
