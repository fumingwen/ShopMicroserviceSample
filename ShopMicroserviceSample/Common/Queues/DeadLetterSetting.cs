using System.Collections.Generic;

namespace Common.Queue
{
    /// <summary>
    /// 死信配置
    /// </summary>
    public class DeadLetterSetting
    {
        /// <summary>
        /// 死信构造函数
        /// </summary>
        /// <param name="persistent">是否持久化，默认true</param>
        /// <param name="exchange">默认用Queue.Dead作为交换器，不能和主队列名称一样</param>
        /// <param name="expiration">过期时间毫秒</param>
        public DeadLetterSetting(bool persistent =true, string exchange="Queue.Dead", int expiration=60000)
        {
            this.Persistent = persistent;
            this.ExchangeName = exchange;
            this.Expiration = expiration;
        }

        /// <summary>
        /// 交换器名称
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// 超时毫秒
        /// </summary>
        public int Expiration { get; set; }
       
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
        /// 死信的死信过期时间
        /// 如果设置此值，队列流转：主队列－死信.Live-死信.Dead
        /// </summary>
        public int DeadQueueExpiration { get; set; }
       
        /// <summary>
        /// 其他参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }
        
        /// <summary>
        /// 双死信，默认配置主队列10天过期，死信队列5分钟后转移到另外一个死信
        /// </summary>
        /// <returns></returns>
        public static DeadLetterSetting DoubleDead()
        {
            return new DeadLetterSetting()
            {
                Expiration = 864000000,
                DeadQueueExpiration = 300000
            };
        }
    }
}
