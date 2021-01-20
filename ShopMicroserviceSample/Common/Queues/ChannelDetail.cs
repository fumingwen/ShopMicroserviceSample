using RabbitMQ.Client;
using System;

namespace Common.Queue
{
    /// <summary>
    /// 频道
    /// </summary>
    internal class ChannelDetail
    {

        /// <summary>
        /// 频道新实例
        /// </summary>
        public ChannelDetail()
        {
            this.BindTime = DateTime.Now;
        }

        /// <summary>
        /// 频道新实例，带参数
        /// </summary>
        public ChannelDetail(IModel model, bool exchangeDeclared)
        {
            this.Channel = model;
            this.ExchangeDeclared = exchangeDeclared;
        }

        /// <summary>
        /// Rabbit频道
        /// </summary>
        public IModel Channel { get; set; }

        /// <summary>
        /// 是否已声明交换器
        /// </summary>
        public bool ExchangeDeclared { get; set; }
       
        /// <summary>
        /// 交换器绑定时间
        /// </summary>
        public DateTime BindTime { get; set; }
    }
}
