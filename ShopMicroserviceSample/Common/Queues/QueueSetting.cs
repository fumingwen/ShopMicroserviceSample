using System.Collections.Generic;

namespace Common.Queue
{
    /// <summary>
    /// 配置参数
    /// </summary>
    public class QueueSetting
    {
        /// <summary>
        /// 队列配置
        /// </summary>
        public QueueSetting()
        {
            this.MatchType = MatchType.Fanout;
            this.Persistent = true;
            this.PrefetchSize = 5;
        }

        /// <summary>
        /// 交换器名称
        /// </summary>
        public string ExchangeName { get; set; }
        
        /// <summary>
        /// 路由键，当交换器名称为空时，routingKey必须为空
        /// </summary>
        public string RoutingKey { get; set; }
       
        /// <summary>
        /// 交换器类型
        /// </summary>
        public MatchType MatchType { get; set; }
      
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Persistent { get; set; }
     
        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; }
        
        /// <summary>
        /// 是否排他队列
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// 不需应答，消息读取后即删除
        /// </summary>
        public bool NoAck { get; set; }

        /// <summary>
        /// 每次取的数据数
        /// </summary>
        public ushort PrefetchSize { get; set; }

        /// <summary>
        /// 当设置为需要应答时，无应答前最多可以获得的信息数
        /// </summary>
        public ushort NoAckCount { get; set; }

        /// <summary>
        /// 是否批量一次性应答所有小于当前消息序号的所有消息
        /// </summary>
        public bool MultipleAck { get; set; }
       
        /// <summary>
        /// 有效期，毫秒，默认0不设置
        /// </summary>
        public int Expiration { get; set; }

        /// <summary>
        /// 其他参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }
        
        /// <summary>
        /// 展示容器
        /// </summary>
        public object Display { get; set; }
     
        /// <summary>
        /// 每条消息接收延迟毫秒数
        /// </summary>
        public int ReceiveDelay { get; set; }

        /// <summary>
        /// 死信配置
        /// </summary>
        public DeadLetterSetting DeadLetter { get; set; }
    }
}
