using System;

namespace Common.Queue
{
    /// <summary>
    /// RabbitMQ 服务器配置
    /// </summary>
    internal class RabbitConfig
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string HostName { get; set; }
       
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
       
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
      
        /// <summary>
        /// 配置时间
        /// </summary>
        public DateTime ConfigTime { get; set; }
       
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Isvalid { get; set; }
    }
}
