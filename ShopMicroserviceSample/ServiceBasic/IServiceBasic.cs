using Common;
using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ServiceBasic
{
    public interface IServiceBasic<Te, Tk, Tm> where Tm : new()
    {
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="entity">对象信息</param>
        /// <returns>返回自增主键</returns>
        OperateResult<Tk> Insert(Te entity);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="identityKey">主键Id</param>
        /// <returns>返回更新记录数</returns>
        OperateResult<int> Delete(Tk identityKey);

        /// <summary>
        /// 根据表达式目录树删除记录
        /// </summary>
        /// <param name="expression">表达式目录树</param>
        /// <returns>变更记录数</returns>
        OperateResult<int> DeleteByCondition(Tm model, Expression<Func<Te, bool>> expression);

        /// <summary>
        /// 根据表达式目录树删除记录
        /// </summary>
        /// <param name="expression">表达式目录树</param>
        /// <returns>变更记录数</returns>
        OperateResult<int> DeleteByCondition(List<Tm> models, Expression<Func<Te, bool>> expression);

        /// <summary>
        /// 根据表达树不分页搜索
        /// </summary>
        /// <param name="expression">搜索请求</param>
        /// <returns>记录列表</returns>
        SearchResult<List<Te>> FindByCondition(Tm model, Expression<Func<Te, bool>> expression);

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity">对象信息</param>
        /// <param name="expression">更新哪些列 i=>new {i.id, i.name}</param>
        /// <returns>返回更新记录数</returns>
        OperateResult<int> Update(Te entity, Expression<Func<Te, dynamic>> expression = null);

        /// <summary>
        /// 批量更新记录
        /// </summary>
        /// <param name="entities">对象列表</param>
        /// <param name="expression">更新哪些列 i=>new {i.id, i.name}</param>
        /// <returns>变更记录数</returns>
        OperateResult<int> BatchUpdate(List<Te> entities, Expression<Func<Te, dynamic>> expression = null);

        /// <summary>
        /// 根据主键Id查找单个记录信息
        /// </summary>
        /// <param name="identityKey">主键Id</param>
        /// <returns>记录信息</returns>
        OperateResult<Te> Find(Tk identityKey);

        /// <summary>
        /// 不分页搜索记录列表
        /// </summary>
        /// <param name="searchRequest">搜索请求</param>
        /// <returns>记录列表结果</returns>
        SearchResult<List<Te>> SearchRequest(SearchRequest<Tm> searchRequest);

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="searchRequest">搜索请求</param>
        /// <returns>记录数</returns>
        OperateResult<int> Count(SearchRequest<Tm> searchRequest);

        /// <summary>
        /// 分页搜索记录列表
        /// </summary>
        /// <param name="pageRequest">分页搜索请求</param>
        /// <returns>分页搜索结果</returns>
        PageResult<List<Te>> PageRequest(PageRequest<Tm> pageRequest);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities">列表</param>
        /// <param name="truncate">插入前是否清空，只有表名带Temp的才有效，不然要主动调Truncate方法</param>
        /// <returns>插入记录数</returns>
        OperateResult<int> BatchInsert(List<Te> entities, bool truncate = false);

        /// <summary>
        /// 不分页搜索全部记录列表
        /// </summary> 
        /// <returns>记录列表结果</returns>
        SearchResult<List<Te>> SearchAllRequest(bool isReload = false);
    }
}
