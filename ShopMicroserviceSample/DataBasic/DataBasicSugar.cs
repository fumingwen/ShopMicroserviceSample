using Dapper;
using Common.Database;
using Common.Tools;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataBasic
{
    /// <summary>
    /// 数据库基本操作
    /// </summary>
    /// <typeparam name="Te">实体对象</typeparam>
    /// <typeparam name="Tk">主键</typeparam>
    /// <typeparam name="Tm">搜索参数</typeparam>
    public abstract class DataBasicSugar<Te, Tk, Tm> : IDataBasic<Te, Tk, Tm> where Te : EntityBase<Tk>, new() where Tm : new()
    {
        protected readonly IDbContext db;

        public TableMapper<Te> TableMapper { get; set; }

        public DataBasicSugar(string configKey)
        {
            db = new DbFactory(configKey).db;
        }

        public DataBasicSugar(string configKey, string defaultKey)
        {
            db = new DbFactory(configKey, defaultKey).db;
        }

        protected SqlSugarClient GetSugarClient()
        {
            SqlSugarClient client = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = db.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });

            client.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + client.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };

            return client;
        }

        #region 通用执行和查询功能

        public int Execute(string sql, object param = null)
        {
            var client = this.GetSugarClient();
            return client.Ado.ExecuteCommand( sql, param);
        }

        public T ExecuteScalar<T>(string sql, object param = null)
        {
            var client = this.GetSugarClient();
            return client.Ado.SqlQuery<T>(sql, param).FirstOrDefault();
        }

        public bool Truncate()
        {
            var sql = $"truncate table {TableMapper.TableName}";
            this.Execute(sql);
            return true;
        }

        public T Query<T>(string sql, object param = null)
        {
            var client = this.GetSugarClient();
            return client.Ado.SqlQuery<T>(sql, param).FirstOrDefault();
        }

        public List<T> Querys<T>(string sql, object param = null)
        {
            var client = this.GetSugarClient();
            return client.Ado.SqlQuery<T>(sql, param);
        }

        #endregion

        #region 具体增删改查

        public Tk Insert(Te entity)
        {
            var client = this.GetSugarClient();
            if (this.TableMapper.InsertReturnIdentity)
            {
                var primaryKey =(Tk)(object) client.Insertable(entity).ExecuteReturnBigIdentity();
                entity.SetPrimaryKey(primaryKey);
                return primaryKey;
            }
            else
            {
                return default;
            }
        }

        public int BatchInsert(List<Te> entities, bool truncate = false)
        {
            if (entities == null || entities.Count < 1) return 0;

            if (truncate && TableMapper.TableName.ToLower().EndsWith("temp"))
            {
                this.Truncate();
            }

            var client = this.GetSugarClient();
            return client.Insertable(entities).ExecuteCommand();
        }

        public int Delete(Tk primaryKey)
        {
            var client = this.GetSugarClient();
            return client.Deleteable<Te>().In(primaryKey).ExecuteCommand();
        }

        public int Delete(params object[] primaryKeys)
        {
            var client = this.GetSugarClient();
            return client.Deleteable<Te>().Where(string.Format(TableMapper.PrimaryKeys, primaryKeys)).ExecuteCommand();
        } 

        public int Update(Te entity, Expression<Func<Te, dynamic>> expression = null)
        {
            var client = this.GetSugarClient();
            return client.Updateable(entity).ExecuteCommand();
        }
        public int BatchUpdate(List<Te> entities, System.Linq.Expressions.Expression<Func<Te, dynamic>> expression = null)
        {
            throw new NotImplementedException();
        }

        public Te Find(Tk primaryKey)
        {
            var client = this.GetSugarClient();
            return client.Queryable<Te>().Where(string.Format(TableMapper.PrimaryKeys, primaryKey)).First();
        }

        public Te Find(params object[] primaryKeys)
        {
            var client = this.GetSugarClient();
            return client.Queryable<Te>().Where(string.Format(TableMapper.PrimaryKeys, primaryKeys)).First();
        }

        #endregion

        #region 通用不分页和分页搜索

        public List<Te> SearchRequest(SearchRequest<Tm> searchRequest)
        {
            var where = this.CombinWhere(searchRequest.AutoWhere(), searchRequest.ManualWhere());
            var sql = string.Format("select top {0} {1} from [{2}] {3}{4}", searchRequest.PageSize, TableMapper.SelectFileds, TableMapper.TableName, TableMapper.ShortTableName, where);
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

        public Queryable<T, T> Queryable<T>(string shortName = null, bool mapColumns = false)
        {
            return new Queryable<T, T>(db, shortName);
        }

        public Queryable<T1, T> Queryable<T1, T>(string shortName = null, bool mapColumns = false)
        {
            return new Queryable<T1, T>(db, shortName);
        }

        public List<Te> FindByCondition(Tm model, Expression<Func<Te, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public int DeleteByCondition(Tm model, Expression<Func<Te, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public int DeleteByCondition(List<Tm> models, Expression<Func<Te, bool>> expression)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
