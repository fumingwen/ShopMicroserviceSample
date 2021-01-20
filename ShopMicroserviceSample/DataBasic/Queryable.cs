using Common.Database;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections.ObjectModel;
using Dapper;
using System.Text.RegularExpressions;
using System.Reflection;
using Common.Helper;

namespace DataBasic
{
    /// <summary>
    /// 高级关联查询类
    /// </summary>
    /// <typeparam name="T1">第一个查询表对象</typeparam>
    /// <typeparam name="T">返回的对象</typeparam>
    public class Queryable<T1, T>
    {
        #region 私有变量
        protected readonly IDbContext db;

        private string columns = null;
        private string shortName = null;

        private StringBuilder joinBuilder = null;
        private StringBuilder whereBuilder = null;
        private StringBuilder groupByBuilder = null;
        private StringBuilder orderByBuilder = null;
        private StringBuilder selectBuilder = null;
        private Dictionary<string, Type> types = null;
        private readonly bool mapColumns = false;
        #endregion

        #region 构造函数
        /// <summary>
        /// 查询构造函数
        /// </summary>
        /// <param name="shortName">第一个表的简称，需要和后面的参数一致</param>
        /// <param name="mapColumns">是否要映射列，如果列名和表字段不一致时就需要</param>
        public Queryable(IDbContext db,  string shortName = null, bool mapColumns = false)
        {
            this.db = db;
            this.shortName = shortName;
            this.mapColumns = mapColumns;
            this.SetType(shortName);
        }
        #endregion

        #region 关联
        /// <summary>
        /// 关联表，最多关联5个
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <param name="expression">关联表达式如：(t1,t2)=>t1.Id=t2.Id</param>
        /// <param name="joinType">关联类型</param>
        /// <returns></returns>
        public Queryable<T1, T> Join<T2>(Expression<Func<T1, T2, bool>> expression, JoinType joinType = JoinType.Inner)
        {
            return this.GetJoin<T2>(joinType, expression, 1);
        }

        /// <summary>
        /// 关联表，最多关联5个
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <param name="expression">关联表达式如：(t1,t2,t3)=>t1.Id=t2.Id and t2.Id=t3.Id</param>
        /// <param name="joinType">关联类型</param>
        /// <returns></returns>
        public Queryable<T1, T> Join<T2, T3>(Expression<Func<T1, T2, T3, bool>> expression, JoinType joinType = JoinType.Inner)
        {
            return this.GetJoin<T3>(joinType, expression, 2);
        }

        private Queryable<T1, T> GetJoin<Tj>(JoinType joinType, Expression expression, int index)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            ReadOnlyCollection<ParameterExpression> parameters = lambda.Parameters;

            this.CheckShortName(parameters[0].ToString());
            if (joinBuilder == null) joinBuilder = new StringBuilder();
            var tjShortName = parameters[index].ToString();
            this.SetType<Tj>(tjShortName);
            string join =ExpressHelper.SqlWhere(lambda.Body);
            joinBuilder.AppendFormat(" {0} join [{1}] {2} on ({3})", joinType.ToString(), ObjectUtil.GetTableName<Tj>(!mapColumns), tjShortName, join);
            return this;
        }

        #endregion

        #region 条件
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="where">where语句，不用and开头</param>
        /// <returns></returns>
        public Queryable<T1, T> Where(string where)
        {
            return this.GetWhere(where);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式如(t1)=>t1.Id=0</param>
        /// <returns></returns>
        public Queryable<T1, T> Where(Expression<Func<T1, bool>> expression)
        {
            return this.GetWhere(expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2)=>t1.Id=0 and t2.Name != null</param>
        /// <returns></returns>
        public Queryable<T1, T> Where<T2>(Expression<Func<T1, T2, bool>> expression)
        {
            return this.GetWhere(expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3)=>t1.Id=0 and t2.Name != null or t3.Type<5</param>
        /// <returns></returns>
        public Queryable<T1, T> Where<T2, T3>(Expression<Func<T1, T2, T3, bool>> expression)
        {
            return this.GetWhere(expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3,t4)=>t1.Id=0 and t2.Name != null or t3.Type<5 and t5.Age in(16,18)</param>
        /// <returns></returns>
        public Queryable<T1, T> Where<T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> expression)
        {
            return this.GetWhere(expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3,t4,t5)=>t1.Id=0 and t2.Name != null or t3.Type<5 and t5.Age in(16,18)...</param>
        /// <returns></returns>
        public Queryable<T1, T> Where<T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> expression)
        {
            return this.GetWhere(expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <typeparam name="T6">第六个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3,t4,t5,t6)=>t1.Id=0 and t2.Name != null or t3.Type<5 and t5.Age in(16,18)...</param>
        /// <returns></returns>
        public Queryable<T1, T> Where<T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> expression)
        {
            return this.GetWhere(expression);
        }
        private Queryable<T1, T> GetWhere(Expression expression)
        {
            string where = ExpressHelper.SqlWhere(expression);
            return this.GetWhere(where);
        }

        private Queryable<T1, T> GetWhere(string where)
        {
            if (whereBuilder == null) whereBuilder = new StringBuilder();
            whereBuilder.AppendFormat(" and ({0})", where);
            return this;
        }
        #endregion

        #region Group by

        /// <summary>
        /// Group by
        /// </summary>
        /// <param name="groupBy">如 t1.id</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy(string groupBy)
        {
            return this.GetGroupBy(groupBy);
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <param name="expression">表达式，如 (t1)=>new {t1.id}</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy(Expression<Func<T1, dynamic>> expression)
        {
            return this.GetGroupBy(expression.Body.ToString());
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2)=>new {t1.id, ... t2.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy<T2>(Expression<Func<T1, T2, dynamic>> expression)
        {
            return this.GetGroupBy(expression.Body.ToString());
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3)=>new {t1.id, ... t3.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy<T2, T3>(Expression<Func<T1, T2, T3, dynamic>> expression)
        {
            return this.GetGroupBy(expression.Body.ToString());
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3,t4)=>new {t1.id, ... t4.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy<T2, T3, T4>(Expression<Func<T1, T2, T3, T4, dynamic>> expression)
        {
            return this.GetGroupBy(expression.Body.ToString());
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3,t4,t5)=>new {t1.id, ... t5.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy<T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, dynamic>> expression)
        {
            return this.GetGroupBy(expression.Body.ToString());
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <typeparam name="T6">第六个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3,t4,t5,t6)=>new {t1.id, ... t6.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> GroupBy<T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, dynamic>> expression)
        {
            return this.GetGroupBy(expression.Body.ToString());
        }

        private Queryable<T1, T> GetGroupBy(string groupBy)
        {
            groupBy = new Regex(@"new <>f__[^\(\)]+\(", RegexOptions.IgnoreCase).Replace(groupBy, "");
            if (groupBy.Substring(0, 1) == "(") groupBy = groupBy.Substring(1);
            if (groupBy.Substring(groupBy.Length - 1, 1) == ")") groupBy = groupBy.Substring(0, groupBy.Length - 1);

            groupBy = string.Format("{{{0}}}", groupBy);

            groupByBuilder = new StringBuilder();
            var bodies = this.FormatDynamicBody(groupBy);
            if (bodies.Length == 1 && !bodies.Contains("="))
            {
                groupByBuilder.Append(bodies[0]);
            }
            else
            {
                foreach (var s in bodies)
                {
                    var temp = s.Split('=');
                    if (temp.Length != 2)
                    {
                        groupByBuilder.AppendFormat("{0}, ", s);
                    }
                    else
                    {
                        groupByBuilder.AppendFormat("{0}, ", temp[1]);
                    }
                }
                groupByBuilder.Append("ENDEND").Replace(", ENDEND", "");
            }

            return this;
        }

        #endregion

        #region 排序
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderBy">order by 字符串，不包括order by</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy(string orderBy)
        {
            return this.GetOrderBy(orderBy);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式如(t1)=> t1.Id或(t1)=> new {t1.Id, OrderByType.DESC, t1.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy(Expression<Func<T1, dynamic>> expression)
        {
            return this.GetOrderBy(expression.Body.ToString());
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2)=> t1.Id或(t1,t2)=> new {t1.Id, OrderByType.DESC, t2.Name}...</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy<T2>(Expression<Func<T1, T2, dynamic>> expression)
        {
            return this.GetOrderBy(expression.Body.ToString());
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3)=> t1.Id或(t1,t2,t3)=> new {t1.Id, OrderByType.DESC, t2.Name}...</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy<T2, T3>(Expression<Func<T1, T2, T3, dynamic>> expression)
        {
            return this.GetOrderBy(expression.Body.ToString());
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3,t4)=> t1.Id或(t1,t2,t3,t4)=> new {t1.Id, OrderByType.DESC, t2.Name}...</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy<T2, T3, T4>(Expression<Func<T1, T2, T3, T4, dynamic>> expression)
        {
            return this.GetOrderBy(expression.Body.ToString());
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3,t4,t5)=> t1.Id或(t1,t2,t3,t4,t5)=> new {t1.Id, OrderByType.DESC, t2.Name}...</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy<T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, dynamic>> expression)
        {
            return this.GetOrderBy(expression.Body.ToString());
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <typeparam name="T6">第六个表对象</typeparam>
        /// <param name="expression">表达式如(t1,t2,t3,t4,t5,t6)=> t1.Id或(t1,t2,t3,t4,t5,t6)=> new {t1.Id, OrderByType.DESC, t2.Name}...</param>
        /// <returns></returns>
        public Queryable<T1, T> OrderBy<T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, dynamic>> expression)
        {
            return this.GetOrderBy(expression.Body.ToString());
        }

        private Queryable<T1, T> GetOrderBy(string orderBy)
        {
            orderByBuilder = new StringBuilder();
            var bodies = this.FormatDynamicBody(orderBy);
            if (bodies.Length == 1 && !bodies.Contains("="))
            {
                orderByBuilder.Append(bodies[0]);
            }
            else
            {
                foreach (var s in bodies)
                {
                    var temp = s.Split('=');
                    if (temp.Length != 2) continue;
                    orderByBuilder.AppendFormat("{0}, ", temp[1]);
                }
                orderByBuilder.Append("ENDEND").Replace(", ENDEND", "").Replace("  "," ").Replace(", DESC", " DESC").Replace(",DESC", " DESC").Replace(", ASC","").Replace(",ASC","");
            }

            return this;
        }
        #endregion

        #region 选字段
        /// <summary>
        /// 选字段
        /// </summary>
        /// <param name="columns">字符串，如 * 或 t1.Name, t2.Id</param>
        /// <returns></returns>
        public Queryable<T1, T> Select(string columns)
        {
            this.columns = columns;
            return this;
        }

        /// <summary>
        /// 选字段
        /// </summary>
        /// <param name="expression">表达式，如 (t1)=>new {t1.id, ... t1.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> Select(Expression<Func<T1, T>> expression)
        {
            return this.GetSelect(expression.Body.ToString());
        }

        /// <summary>
        /// 选字段
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2)=>new {t1.id, ... t2.Name}</param>

        public Queryable<T1, T> Select<T2>(Expression<Func<T1, T2, T>> expression)
        {
            return this.GetSelect(expression.Body.ToString());
        }

        /// <summary>
        /// 选字段
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3)=>new {t1.id, ... t3.Name}</param>
        public Queryable<T1, T> Select<T2, T3>(Expression<Func<T1, T2, T3, T>> expression)
        {
            return this.GetSelect(expression.Body.ToString());
        }

        /// <summary>
        /// 选字段
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3,t4)=>new {t1.id, ... t4.Name}</param>
        public Queryable<T1, T> Select<T2, T3, T4>(Expression<Func<T1, T2, T3, T4, T>> expression)
        {
            return this.GetSelect(expression.Body.ToString());
        }

        /// <summary>
        /// 选字段
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3,t4,t5)=>new {t1.id, ... t5.Name}</param>
        public Queryable<T1, T> Select<T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, T>> expression)
        {
            return this.GetSelect(expression.Body.ToString());
        }

        /// <summary>
        /// 选字段
        /// </summary>
        /// <typeparam name="T2">第二个表对象</typeparam>
        /// <typeparam name="T3">第三个表对象</typeparam>
        /// <typeparam name="T4">第四个表对象</typeparam>
        /// <typeparam name="T5">第五个表对象</typeparam>
        /// <typeparam name="T6">第六个表对象</typeparam>
        /// <param name="expression">表达式，如 (t1,t2,t3,t4,t5,t6)=>new {t1.id, ... t6.Name}</param>
        /// <returns></returns>
        public Queryable<T1, T> Select<T2, T3, T4, T5, T6>(Expression<Func<T1, T2, T3, T4, T5, T6, T>> expression)
        {
            return this.GetSelect(expression.Body.ToString());
        }

        private Queryable<T1, T> GetSelect(string select)
        {
            columns = null;
            select = new Regex(@"value\([^\(\)]+\)\.", RegexOptions.IgnoreCase).Replace(select, "");
            if (selectBuilder == null) selectBuilder = new StringBuilder();
            selectBuilder.Append(select);
            return this;
        }
        #endregion

        #region 返回结果

        public List<T> ToList(int pageSize = 0)
        {
            var top = pageSize > 0 ? string.Format(" top {0}", pageSize) : "";
            var sql = string.Format("select{0} {1}", top, this.GetSql());
            var connection = db.CreateConnection();
            var result = connection.Query<T>(sql, null).ToList();
            db.CloseConnection(connection);
            return result;
        }

        public List<T> ToList<TModel>(SearchRequest<TModel> searchRequest) where TModel : new()
        {
            this.InsertSearch(searchRequest);
            return this.ToList(searchRequest.PageSize);
        }

        public PageResult<List<T>> ToPage(int pageSize = 10, int currentPage = 1)
        {
            var pageRequest = new PageRequest<int>(0, null, pageSize, currentPage);
            var originalSql = this.GetSql();
            var countSql = string.Format("select count(0) {0}", originalSql.Substring(originalSql.IndexOf("from")));
            var tempSql = countSql.Split(" order by");

            var connection = db.CreateConnection();
            int totalCount = connection.Query<int>(tempSql[0]).FirstOrDefault();
            if (totalCount < 1) return new PageResult<List<T>>(default, 0, pageRequest.PageSize, pageRequest.CurrentPage);

            string orderBy;
            if (tempSql.Length > 1)
            {
                orderBy = tempSql[tempSql.Length - 1];
            }
            else
            {
                orderBy = originalSql.Substring(0, originalSql.IndexOf(","));
                var asIndex = orderBy.IndexOf(" as ");
                if (asIndex > -1) orderBy = orderBy.Substring(asIndex);
            }

            pageRequest.CheckPage(totalCount);

            var top = pageRequest.CurrentPage * pageRequest.PageSize;
            var indexFrom = top - pageRequest.PageSize + 1;
            var indexEnd = top;

            var columnsSb = new StringBuilder();
            var columnsSql = originalSql.Substring(0, originalSql.IndexOf("from"));
            var temp = columnsSql.Split(",");
            foreach (var s in temp)
            {
                var index = s.IndexOf(" as ");
                if (index > 0)
                {
                    columnsSb.AppendFormat("{0}, ", s.Substring(index + 4));
                }
                else
                {
                    index = s.IndexOf(".");
                    if (index < 0)
                    {
                        columnsSb.AppendFormat("{0}, ", s);
                    }
                    else
                    {
                        columnsSb.AppendFormat("{0}, ", s.Substring(index + 1));
                    }
                }
            }
            columnsSb.Append("ENDEND").Replace(", ENDEND", "");

            var sql = string.Format("select {0} from (select top {1} ROW_NUMBER() over( order by {2}) as RowNumber, {3}) t where RowNumber between {4} and {5}", columnsSb.ToString(), top, orderBy, originalSql, indexFrom, indexEnd);
            var list = connection.Query<T>(sql).ToList();

            db.CloseConnection(connection);
            return new PageResult<List<T>>(list, totalCount, pageRequest.PageSize, pageRequest.CurrentPage);
        }

        public PageResult<List<T>> ToPage<TModel>(PageRequest<TModel> pageRequest) where TModel: new ()
        {
            this.InsertSearch(pageRequest);
            return this.ToPage(pageRequest.PageSize, pageRequest.CurrentPage);
        }

        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <returns></returns>
        public T Single()
        {
            var sql =string.Format("select top 1 {0}", this.GetSql());

            var connection = db.CreateConnection();
            var result = connection.Query<T>(sql).FirstOrDefault();
            db.CloseConnection(connection);
            return result;
        }
        #endregion

        #region 其他私有函数
        private void SetType(string name)
        {
            this.SetType<T1>(name);
        }

        private void SetType<Tn>(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (types == null) types = new Dictionary<string, Type>();
            if(!types.ContainsKey(name)) types.Add(name, typeof(Tn));
        }

        private void CheckShortName(string newName)
        {
            if (string.IsNullOrEmpty(this.shortName))
            {
                shortName = newName;
                this.SetType(shortName);
                return;
            }

            if (this.shortName != newName)
            {
                throw new Exception($"第一个表的简称{this.shortName}与{newName}不对应");
            }
        }

        private void InsertSearch<TModel>(SearchRequest<TModel> searchRequest) where TModel : new()
        {
            var where = string.Format("{0}{1}", searchRequest.AutoWhere(), searchRequest.ManualWhere());
            if (!string.IsNullOrEmpty(where))
            {
                if (whereBuilder == null) whereBuilder = new StringBuilder();
                whereBuilder.Insert(0, string.Format(" and ({0})", where.Substring(5)));
            }

            var order = searchRequest.OrderBy;
            if (!string.IsNullOrEmpty(order))
            {
                if (orderByBuilder == null)
                {
                    orderByBuilder = new StringBuilder();
                    orderByBuilder.Append(order);
                }
                else
                {
                    orderByBuilder.Insert(0, searchRequest.OrderBy + ", ");
                }
            }
        }

        private string GetSql()
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(columns))
            {
                if (selectBuilder != null)
                {
                    columns = this.FormatSelect(selectBuilder);
                }
                if (string.IsNullOrEmpty(columns))
                {
                    if (types == null || types.Count < 2 && !mapColumns)
                    {
                        columns = "*";
                    }
                    else
                    {
                        columns = ObjectUtil.GetTableColumns<T>();
                    }
                }
            }
            sb.AppendFormat("{0} from [{1}] {2}", columns, ObjectUtil.GetTableName<T1>(!mapColumns), this.shortName);

            if (joinBuilder != null)
            {
                sb.AppendFormat(" {0}", this.FormatWhere(joinBuilder));
            }

            if (whereBuilder != null)
            {
                sb.AppendFormat(" where 1=1{0}", this.FormatWhere(whereBuilder)).Replace(" where 1=1 and", " where");
            }

            if (groupByBuilder != null)
            {
                sb.AppendFormat(" group by {0}", groupByBuilder);
            }

            if (orderByBuilder != null)
            {
                sb.AppendFormat(" order by {0}", orderByBuilder.Replace("[","").Replace("]","").ToString());
            }

            var sql = sb.ToString();
            if (!mapColumns)
            {
                return sql;
            }
            else
            {
                return MapColumns(sql);
            }
        }

        private string MapColumns(string sql)
        {
            string regstr = @"((\s)||(\p{P}))(\w+)\.(\w+)((\s)||(\p{P}))";
            Regex reg = new Regex(regstr, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(sql);
            foreach (Match m in mc)
            {
                var value = m.Value.Replace("(", "").Replace(",", "").Replace(")", "").Replace(" ", "");
                var temp = value.Split(".");
                if (temp.Length !=2) continue;

                var success=types.TryGetValue(temp[0], out Type type);
                if (!success || type == null) continue;

                var property=type.GetProperty(temp[1]);

                if (property == null) continue;

                var filedComment = (FiledComment)property.GetCustomAttribute(typeof(FiledComment));
                if (filedComment != null)
                {
                    var columnName = filedComment.ColumnName;
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        sql = sql.Replace($"{temp[0]}.{temp[1]}", $"{temp[0]}.[{columnName}]");
                    }
                }
            }
            return sql;
        }

        private string FormatWhere(StringBuilder sb)
        {
            var where = sb.Replace("AndAlso", "and").Replace("==", "=").Replace("!=null", "is not null").Replace("!= null", "is not null").Replace("=null", " is null").Replace("= null", " is null").ToString();
            return where;
        }
       
        private string[] FormatDynamicBody(string body)
        {
            var bodies = body.Split('{');
            if (bodies.Length < 2) return new string[] { body };

            body = bodies[1].Replace("}","");

            bodies = body.Split(",");

            return bodies;
        }

        private string FormatSelect(StringBuilder sb)
        {
            var bodies = this.FormatDynamicBody(sb.ToString());
            if (bodies == null || bodies.Length < 1) return null;

            var newSb = new StringBuilder();
            if (bodies.Length == 1 && !bodies.Contains("="))
            {
                newSb.Append(bodies[0]);
            }
            else
            {
                foreach (var s in bodies)
                {
                    var temp = s.Split('=');
                    if (temp.Length != 2) continue;
                    newSb.AppendFormat("{0} as {1}, ", temp[1], temp[0]);
                }
                newSb.Append("ENDEND").Replace(", ENDEND", "");
            }

            return newSb.ToString();
        }
        #endregion
    }
}
