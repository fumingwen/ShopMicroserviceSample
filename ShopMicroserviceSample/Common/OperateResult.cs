using Common.Enums;
using System;

namespace Common 
{
	/// <summary>
	/// 操作结果
	/// </summary>
	/// <typeparam name="T">返回数据类型，泛型</typeparam>
	public class OperateResult<T>
	{
		/// <summary>
		/// 返回数据
		/// </summary>

		public T Data { get; set; }

		/// <summary>
		/// 操作状态，OperateStatus枚举
		/// </summary>
		public OperateStatus OperateStatus { get; set; }

		/// <summary>
		/// 返回信息
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 返回代码
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 其他辅助参数，Json格式
		/// </summary>
		public string OtherJson { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public OperateResult()
		{

		}

		/// <summary>
		/// 实例化操作结果
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="operateStatus">操作状态</param>
		/// <param name="message">返回信息</param>
		/// <param name="code">返回代码</param>
		/// <param name="otherJson">其他辅助参数，Json格式</param>
		public OperateResult(T data = default, OperateStatus operateStatus = OperateStatus.Unknow, string message = null, string code = null, string otherJson = null)
		{
			this.Data = data;
			this.OperateStatus = operateStatus;
			this.Code = code;
			this.Message = message;
			this.OtherJson = otherJson;
		}

		/// <summary>
		/// 是否成功
		/// </summary>
		/// <returns></returns>
		public bool Ok()
		{
			return this.OperateStatus.Equals(OperateStatus.Success);
		}

		/// <summary>
		/// 是否未知
		/// </summary>
		/// <returns></returns>
		public bool Unsure()
		{
			return this.OperateStatus.Equals(OperateStatus.Unknow);
		}

		/// <summary>
		/// 操作成功
		/// </summary>
		/// <returns></returns>
		public static OperateResult<T> Success()
		{
			return Success(default);
		}

		/// <summary>
		/// 操作成功
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="message">返回信息</param>
		/// <param name="code">返回代码</param>
		/// <param name="otherJson">其他辅助参数，Json格式</param>
		/// <returns></returns>
		public static OperateResult<T> Success(T data, string message = null, string code = null, string otherJson = null)
		{
			return new OperateResult<T>(data, OperateStatus.Success, message, code, otherJson);
		}

		/// <summary>
		/// 操作失败
		/// </summary>
		/// <param name="message">返回信息</param>
		/// <param name="code">返回代码</param>
		/// <param name="otherJson">其他辅助参数，Json格式</param>
		/// <returns></returns>
		public static OperateResult<T> Failed(string message, string code = null, string otherJson = null)
		{
			return new OperateResult<T>(default, OperateStatus.Failed, message, code, otherJson);
		}

		/// <summary>
		/// 返回其他Json信息
		/// </summary>
		/// <param name="otherJson"></param>
		/// <returns></returns>
		public OperateResult<T> SetOtherJson(string otherJson)
		{
			this.OtherJson = otherJson;
			return this;
		}
	}
}
