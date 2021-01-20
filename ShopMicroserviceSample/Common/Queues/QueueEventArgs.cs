using Common.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Queue
{

    /// <summary>
    /// 消息事件
    /// </summary>
    public class QueueEventArgs: EventArgs
    {
        /// <summary>
        /// 消息事件
        /// </summary>
        /// <param name="index">消息序列</param>
        /// <param name="body">消息内容</param>
        public QueueEventArgs(ulong index, byte[] body)
        {
            this.Index = index;
            this.Message = Encoding.Default.GetString(body);
        }

        /// <summary>
        /// 消息事件
        /// </summary>
        /// <param name="index">消息序列</param>
        /// <param name="body">消息内容</param>
        /// <param name="display">展示容器</param>
        public QueueEventArgs(ulong index, byte[] body, object display)
        {
            this.Index = index;
            this.Message = Encoding.Default.GetString(body);
        }

        /// <summary>
        /// 消息序列
        /// </summary>
        public ulong Index { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 展示容器
        /// </summary>
        public object Display { get; set; }

        /// <summary>
        /// 将消息Json形式转换为实体对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public T ToObject<T>()
        {
            var type = typeof(T).ToString().ToLower();
            if (Constants.BasicTypes.Contains(type)) return (T)(object)Message;
            return JsonUtil.FromJson<T>(Message);
        }
    }
}