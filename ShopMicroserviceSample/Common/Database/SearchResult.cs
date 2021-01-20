using System;
using Common.Enums;
using Common.Tools;

namespace Common.Database
{
	/// <summary>
	/// 通用搜索结果
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class SearchResult<T> : OperateResult<T> where T : new()
	{
		/// <summary>
		/// 记录数
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// 搜索结果
		/// </summary>
		public SearchResult() : base(default, OperateStatus.Success)
		{
			TotalCount = 0;
		}

		/// <summary>
		/// 搜索结果
		/// </summary>
		/// <param name="data"></param>
		/// <param name="totalCount"></param>
		public SearchResult(T data, int totalCount = 0) : base(data, OperateStatus.Success)
		{
			this.TotalCount = totalCount;
		}

		/// <summary>
		/// 搜索结果
		/// </summary>
		/// <param name="data"></param>
		/// <param name="message"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public SearchResult<T> Success(T data, string message = null, string code = null)
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
		/// <param name="message">消息</param>
		/// <param name="code">错误代码</param>
		/// <returns></returns>
		public SearchResult<T> Failed(string message, string code = null)
		{
			this.OperateStatus = OperateStatus.Failed;
			this.Message = message;
			this.Code = code;
			return this;
		}

		/// <summary>
		/// 转换为Json
		/// </summary>
		/// <returns></returns>
		public string ToJson()
		{
			return JsonUtil.ToJson(this);
		}
	}
}
