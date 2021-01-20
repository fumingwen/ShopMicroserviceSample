using StackExchange.Redis;
using System.Collections.Generic;

namespace Common.Caches
{
    public class RedisConfig
    {
        /// <summary>
        /// Ip和端口列表
        /// </summary>
        public List<string> EndPoints { get; set; }

        /// <summary>
        /// 名称，用来标识连接选项
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 超级管理权限,false
        /// </summary>
        public bool AllowAdmin { get; set; }

        /// <summary>
        /// 连接重试次数,3
        /// </summary>
        public int ConnectRetry { get; set; }

        /// <summary>
        /// 连接超时时间,毫秒10000
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 默认数据库序号,0
        /// </summary>
        public int DatabaseIndex { get; set; }

        /// <summary>
        /// 是否加密,默认false
        /// </summary>
        public bool Ssl { get; set; }

        /// <summary>
        /// 连接超时是否抛出异常,true
        /// </summary>
        public bool AbortOnConnectFail { get; set; }

        public ConfigurationOptions CreateConfigurationOptions()
        {
            var option = new ConfigurationOptions()
            {
                ClientName = this.ClientName,
                AllowAdmin = this.AllowAdmin,
                ConnectRetry = this.ConnectRetry,
                ConnectTimeout = this.ConnectTimeout,
                Password = this.Password,
                DefaultDatabase = this.DatabaseIndex,
                Ssl = this.Ssl,
                AbortOnConnectFail = this.AbortOnConnectFail
            };
            foreach (var s in this.EndPoints)
            {
                option.EndPoints.Add(s);
            }
            return option;
        }
    }
}
