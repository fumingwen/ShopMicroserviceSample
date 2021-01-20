using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Common.Database;

namespace DataBasic
{
    /// <summary>
    /// 数据库基本操作
    /// </summary>
    /// <typeparam name="Te">实体对象</typeparam>
    /// <typeparam name="Tk">主键</typeparam>
    /// <typeparam name="Tm">搜索参数</typeparam>
    public interface IDataBasic<Te, Tk, Tm> where Te : EntityBase<Tk>, new() where Tm : new()
    {
        /// <summary>
        /// 查询单个数据对象
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="sql">执行的sql语句</param>
        /// <param name="param">如果sql语句参数化需要传入对象结构</param>
        /// <returns>筛选的数据对象</returns>
        T Query<T>(string sql, object param = null);

        /// <summary>
        /// 查询数据对象列表
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="sql">执行的sql语句</param>
        /// <param name="param">如果sql语句参数化需要传入对象结构</param>
        /// <returns>筛选的数据对象列表</returns>
        List<T> Querys<T>(string sql, object param = null);

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns></returns>
        Tk Insert(Te entity);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities">对象列表</param>
        /// <param name="truncate">插入前是否清空，只有表名带Temp的才有效，不然要主动调Truncate方法</param>
        /// <returns>插入记录数</returns>
        int BatchInsert(List<Te> entities, bool truncate = false);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="primaryKey">主键</param>
        /// <returns>变更记录数</returns>
        int Delete(Tk primaryKey);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="primaryKeys">主键组合</param>
        /// <returns>变更记录数</returns>
        int Delete(params object[] primaryKeys);

        /// <summary>
        /// 根据表达式目录树删除记录
        /// </summary>
        /// <param name="expression">表达式目录树</param>
        /// <returns>变更记录数</returns>
        int DeleteByCondition(Tm model, Expression<Func<Te, bool>> expression);

        /// <summary>
        /// 根据表达式目录树删除记录
        /// </summary>
        /// <param name="expression">表达式目录树</param>
        /// <returns>变更记录数</returns>
        int DeleteByCondition(List<Tm> models, Expression<Func<Te, bool>> expression);

        /// <summary>
        /// 根据表达树不分页搜索
        /// </summary>
        /// <param name="expression">搜索请求</param>
        /// <returns>记录列表</returns>
        List<Te> FindByCondition(Tm model, Expression<Func<Te, bool>> expression);

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="expression">更新哪些列 i=>new {i.id, i.name}</param>
        /// <returns>变更记录数</returns>
        int Update(Te entity, Expression<Func<Te, dynamic>> expression = null);

        /// <summary>
        /// 批量更新记录
        /// </summary>
        /// <param name="entities">对象列表</param>
        /// <param name="expression">更新哪些列 i=>new {i.id, i.name}</param>
        /// <returns>变更记录数</returns>
        int BatchUpdate(List<Te> entities, Expression<Func<Te, dynamic>> expression = null);

        /// <summary>
        /// 根据主键查找单条记录
        /// </summary>
        /// <param name="primaryKey">主键</param>
        /// <returns>对象实体</returns>
        Te Find(Tk primaryKey);
        
        /// <summary>
        /// 根据主键组合查找单条记录
        /// </summary>
        /// <param name="primaryKeys">主键组合</param>
        /// <returns>对象实体</returns>
        Te Find(params object[] primaryKeys);

        /// <summary>
        /// 不分页搜索
        /// </summary>
        /// <param name="searchRequest">搜索请求</param>
        /// <returns>记录列表</returns>
        List<Te> SearchRequest(SearchRequest<Tm> searchRequest);

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="searchRequest">请求记录</param>
        /// <returns>记录数</returns>
        int Count(SearchRequest<Tm> searchRequest);

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <param name="pageRequest">分页请求记录</param>
        /// <returns>分页记录信息</returns>
        PageResult<List<Te>> PageRequest(PageRequest<Tm> pageRequest);
    }
}
