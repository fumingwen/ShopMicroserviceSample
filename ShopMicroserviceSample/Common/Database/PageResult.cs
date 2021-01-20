using System;
using Common.Enums;
using Common.Tools;

namespace Common.Database
{
	/// <summary>
	/// 分页搜索结果
	/// </summary>
	/// <typeparam name="T">数据列表</typeparam>
	public class PageResult<T> : SearchResult<T> where T : new()
	{
		/// <summary>
		/// 每页记录条数
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// 当前页
		/// </summary>
		public int CurrentPage { get; set; }

		public PageResult() : base()
		{

		}

		public PageResult(T data, int totalCount, int pageSize, int currentPage) : base(data, totalCount)
		{
			this.PageSize = pageSize;
			this.CurrentPage = currentPage;
		}

		/// <summary>
		/// 总页数
		/// </summary>
		public int TotalPage
		{
			get
			{
				if (this.PageSize < 1)
				{
					this.PageSize = 10;
				}
				return (int)Math.Ceiling((double)this.TotalCount / (double)this.PageSize);
			}
		}

		/// <summary>
		/// 成功
		/// </summary>
		/// <param name="data"></param>
		/// <param name="message"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public new PageResult<T> Success(T data, string message=null, string code = null)
		{
			this.OperateStatus = OperateStatus.Success;
			this.Data = data;
			this.Message = message;
			this.Code = code;
			return this;
		}

		/// <summary>
		/// 失败
		/// </summary>
		/// <param name="message"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public new PageResult<T> Failed(string message, string code = null)
		{
			this.OperateStatus = OperateStatus.Failed;
			this.Message = message;
			this.Code = code;
			return this;
		}
	}
	
}
