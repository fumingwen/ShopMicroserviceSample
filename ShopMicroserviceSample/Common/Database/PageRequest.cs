using System;

namespace Common.Database
{
	/// <summary>
	/// 通用分页搜索请求
	/// </summary>
	/// <typeparam name="T">搜索参数</typeparam>
	public class PageRequest<T> : SearchRequest<T> where T : new()
	{
		/// <summary>
		/// 当前页
		/// </summary>
		public int CurrentPage { get; set; }

		/// <summary>
		/// 分页搜索请求
		/// </summary>
		public PageRequest() : base()
		{

		}

		/// <summary>
		/// 分页搜索请求
		/// </summary>
		/// <param name="model">搜索参数</param>
		public PageRequest(T model) : base(model)
		{

		}

		/// <summary>
		/// 分页搜索请求
		/// </summary>
		/// <param name="model">搜索参数</param>
		/// <param name="orderBy">排序</param>
		/// <param name="pageSize">每页记录数</param>
		/// <param name="currentPage">当前页</param>
		public PageRequest(T model, string orderBy, int pageSize, int currentPage) : base(model, orderBy, pageSize)
		{
			this.CurrentPage = currentPage;
		}

		/// <summary>
		/// 检测分页
		/// </summary>
		/// <param name="totalCount"></param>
		public void CheckPage(int totalCount)
		{
			if (this.PageSize < 1)
			{
				this.PageSize = 10;
			}

			int totalPage = (int)Math.Ceiling((double)totalCount / (double)this.PageSize);

			if (this.CurrentPage == 0)
			{
				this.CurrentPage = 1;
			}
			if (this.CurrentPage > totalPage)
			{
				//this.setCurrentPage(totalPage);
			}

			if (this.CurrentPage < 1)
			{
				this.CurrentPage = 1;
			}
		}
	}
}
