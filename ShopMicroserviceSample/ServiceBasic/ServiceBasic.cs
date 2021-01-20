using Common;
using Common.Caches;
using Common.Database;
using DataBasic;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceBasic
{
    public abstract class ServiceBasic<Te, Tk, Tm, IData> : IServiceBasic<Te, Tk, Tm> where Te : EntityBase<Tk>, new() where Tm : new() where IData : IDataBasic<Te, Tk, Tm>
    {
        private readonly IData data;
        private readonly ICache cache;

        public ServiceBasic(IData data)
        {
            this.data = data;
        }
        public ServiceBasic(IData data, ICache cache)
        {
            this.data = data;
            this.cache = cache;
        }

        public OperateResult<Tk> Insert(Te entity)
        {
            try
            {
                var identityKey = data.Insert(entity);
                if (long.Parse(identityKey.ToString()) > 0)
                {
                    return OperateResult<Tk>.Success(identityKey);
                }
                else
                {
                    return OperateResult<Tk>.Failed(default, "数据重复或不符合插入规则");
                }
            }
            catch (Exception ex)
            {
                return OperateResult<Tk>.Failed(ex.Message);
            }
        }

        public OperateResult<int> Delete(Tk identityKey)
        {
            try
            {
                var count = data.Delete(identityKey);
                return OperateResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        public OperateResult<int> DeleteByCondition(Tm model, Expression<Func<Te, bool>> expression)
        {
            try
            {
                var count = data.DeleteByCondition(model, expression);
                return OperateResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        public OperateResult<int> DeleteByCondition(List<Tm> models, Expression<Func<Te, bool>> expression)
        {
            try
            {
                var count = data.DeleteByCondition(models, expression);
                return OperateResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        public SearchResult<List<Te>> FindByCondition(Tm model, Expression<Func<Te, bool>> expression)
        {
            try
            {
                var result = data.FindByCondition(model, expression);
                return this.SearchResult(result);
            }
            catch (Exception ex)
            {
                return new SearchResult<List<Te>>().Failed(ex.Message);
            }
        }

        public OperateResult<int> Update(Te entity, Expression<Func<Te, dynamic>> expression = null)
        {
            try
            {
                var count = data.Update(entity, expression);
                return OperateResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        public OperateResult<int> BatchUpdate(List<Te> entities, Expression<Func<Te, dynamic>> expression)
        {
            try
            {
                var count = data.BatchUpdate(entities, expression);
                return OperateResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        public OperateResult<Te> Find(Tk identityKey)
        {
            try
            {
                var entity = data.Find(identityKey);
                return OperateResult<Te>.Success(entity);
            }
            catch (Exception ex)
            {
                return OperateResult<Te>.Failed(ex.Message);
            }
        }

        public SearchResult<List<Te>> SearchRequest(SearchRequest<Tm> searchRequest)
        {
            try
            {
                var result = data.SearchRequest(searchRequest);
                return this.SearchResult(result);
            }
            catch (Exception ex)
            {
                return new SearchResult<List<Te>>().Failed(ex.Message);
            }
        }

        public OperateResult<int> Count(SearchRequest<Tm> searchRequest)
        {
            try
            {
                var result = data.Count(searchRequest);
                return OperateResult<int>.Success(result);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        public PageResult<List<Te>> PageRequest(PageRequest<Tm> pageRequest)
        {
            try
            {
                var result = data.PageRequest(pageRequest);
                return result;
            }
            catch (Exception ex)
            {
                return new PageResult<List<Te>>().Failed(ex.Message);
            }
        }

        public OperateResult<int> BatchInsert(List<Te> entities, bool truncate = false)
        {
            try
            {
                var count = data.BatchInsert(entities, truncate);
                return OperateResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return OperateResult<int>.Failed(ex.Message);
            }
        }

        protected SearchResult<List<T>> SearchResult<T>(List<T> data)
        {
            var totalCount = 0;
            if (data != null && data.Count > 0) totalCount = data.Count;
            return new SearchResult<List<T>>(data, totalCount);
        }

        public SearchResult<List<Te>> SearchAllRequest(bool isReload = false)
        {
            try
            {
                Type type = typeof(Te);
                var cacheResult = cache.Get<List<Te>>($"{type}");
                var result = new List<Te>();

                if (isReload || !(cacheResult != null && cacheResult.Count > 0))
                {
                    result = data.SearchRequest(new SearchRequest<Tm>(new Tm()) { PageSize = -1 });
                    Task.Run(() => cache.Add($"{type}", result, 300));
                }
                else
                {
                    result = cacheResult;
                }

                return this.SearchResult(result);
            }
            catch (Exception ex)
            {
                return new SearchResult<List<Te>>().Failed(ex.Message);
            }
        }
    }
}
