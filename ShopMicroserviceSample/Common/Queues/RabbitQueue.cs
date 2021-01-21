using Common.Helper;
using Common.Tools;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Queue
{
    /// <summary>
    ///  RabbitMQ实现类
    /// </summary>
    public class RabbitQueue : IQueue, IDisposable
    {
        #region 私有变量和构造函数

        private readonly string configName = "Default";
        private static RabbitConfig config = null;

        private IConnection publishConnection = null;
        private IConnection readConnection = null;

        private readonly ConcurrentDictionary<string, ChannelDetail> channelDetails = new ConcurrentDictionary<string, ChannelDetail>();

        private bool disposedValue = false; 

        /// <summary>
        /// 事件处理
        /// </summary>
        public event EventHandler<QueueEventArgs> QueueHanlder;

        /// <summary>
        /// 消息队列构造函数
        /// </summary>
        /// <param name="configName">服务配置名称</param>
        public RabbitQueue(string configName = "Default")
        {
            this.configName = configName;
            this.LoadConfig();
        }

        private void LoadConfig()
        { 
            if (config != null) return;

            config = new RabbitConfig()
            {
                HostName = ConfigurtaionHelper.Configuration[$"RabbitMQ:HostName"],
                UserName = ConfigurtaionHelper.Configuration[$"RabbitMQ:UserName"],
                Password = ConfigurtaionHelper.Configuration[$"RabbitMQ:Password"],
                ConfigName = ConfigurtaionHelper.Configuration[$"RabbitMQ:ConfigName"],
                Port = int.Parse(ConfigurtaionHelper.Configuration[$"RabbitMQ:Port"]),
                ConfigTime = DateTime.Parse(ConfigurtaionHelper.Configuration[$"RabbitMQ:ConfigTime"]),
                Isvalid = bool.Parse(ConfigurtaionHelper.Configuration[$"RabbitMQ:Isvalid"])
            };  
        }

        private ConnectionFactory CreateFactory()
        {
            this.LoadConfig();
            return new ConnectionFactory()
            {
                HostName = config.HostName,
                UserName = config.UserName,
                Password = config.Password
            };
        }

        private void CreateConnection(ConnectionFactory factory, ChannelType channelType)
        {
            if (channelType == ChannelType.Publish)
            {
                publishConnection = factory.CreateConnection();
            }
            else
            {
                readConnection = factory.CreateConnection();
            }
        }

        /// <summary>
        /// 初始化服务器配置
        /// </summary>
        private void Initial(ChannelType channelType)
        {
            var factory = CreateFactory();
            this.CreateConnection(factory, channelType);
        }

        #endregion

        #region 底层操作Rabbit

        /// <summary>
        /// 声明频道
        /// </summary>
        /// <param name="name">频道名称，可以和队列名称一致</param>
        /// <param name="channelType">频道类型：0写，1读</param>
        /// <returns>频道</returns>
        private ChannelDetail CreateChannelDetail(string name, ChannelType channelType = ChannelType.Publish)
        {
            //选择连接
            var conn = GetConnection(channelType);

            //如果频道已存在，直接读取
            if (channelDetails.ContainsKey(name)) return channelDetails[name];

            //声明新的频道
            var channel = conn.CreateModel();

            //将新的频道缓存并返回
            return channelDetails.GetOrAdd(name, key =>
            {
                return new ChannelDetail(channel, false);
            });
        }

        /// <summary>
        ///  声明交换器
        /// </summary>
        /// <param name="channel">已有频道</param>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="type">交换器类型： 
        /// 1、Direct Exchange，需要将一个队列绑定到交换机上，并且路由键完全匹配；
        /// 2、Fanout Exchange，只需要简单的将队列绑定到交换机上，不要求路由键匹配，Fanout交换机转发消息是最快的；
        /// 3、Topic Exchange，需要队列绑定到一个模式上，符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词，
        /// 因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*”只会匹配到“audit.irs”。
        /// </param>
        /// <param name="persistent">是否持久化</param>
        /// <param name="autoDelete">是否自动删除</param>
        /// <param name="arguments">其他参数</param>
        private void CreateExchange(IModel channel, string exchangeName, string type = ExchangeType.Fanout, bool persistent = true, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            if (string.IsNullOrEmpty(exchangeName)) return;
            channel.ExchangeDeclare(exchangeName, type.ToLower(), persistent, autoDelete, arguments);
        }

        /// <summary>
        /// 声明队列
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="persistent">是否持久化</param>
        /// <param name="exclusive">是否排他队列，如果一个队列被声明为排他队列，该队列仅对首次声明它的连接可见，
        /// 并在连接断开时自动删除。这里需要注意三点：其一，排他队列是基于连接可见的，同一连接的不同信道是可
        /// 以同时访问同一个连接声明的排他队列的。其二，“首次”，如果一个连接已经声明了一个排他队列，其他连
        /// 接是不允许建立同名的排他队列的，这个与普通队列不同。其三，即使该队列是持久化的，一旦连接关闭或者
        /// 客户端退出，该排他队列都会被自动删除的。这种队列适用于只限于一个客户端发送读取消息的应用场景。</param>
        /// <param name="autoDelete">是否自动删除</param>
        /// <param name="arguments">其他参数</param>
        private void CreateQueue(IModel channel, string queueName, bool persistent = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            if (string.IsNullOrEmpty(queueName)) return;
            channel.QueueDeclare(queueName, persistent, exclusive, autoDelete, arguments);
        }

        /// <summary>
        ///  绑定队列和交换器
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="routingKey">路由键</param>
        private void BindQueue(IModel channel, string exchangeName, string queueName, string routingKey)
        {
            if (!string.IsNullOrEmpty(exchangeName) && !string.IsNullOrEmpty(routingKey))
            {
                channel.QueueBind(queueName, exchangeName, routingKey);
            }
        }

        /// <summary>
        /// 根据发布或读取的性质选择连接
        /// </summary>
        /// <param name="channelType">频道类型：0发布，1读取</param>
        /// <returns>连接</returns>
        private IConnection GetConnection(ChannelType channelType)
        {
            var connection = (channelType == ChannelType.Publish ? publishConnection : readConnection);
            if (connection == null || !connection.IsOpen)
            {
                Initial(channelType);
                connection = (channelType == ChannelType.Publish ? publishConnection : readConnection);
            }
            return connection;
        }

        /// <summary>
        /// 发布消息队列
        /// </summary>
        /// <param name="queueName">队列名</param>
        /// <param name="message">队列信息</param>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="exchangeType">交换器类型：Direct完全匹配、Fanout不匹配、Topic模式匹配</param>
        /// <param name="routingKey">路由键，当交换器名称为空时，routingKey必须为空</param>
        /// <param name="persistent">是否持久化</param>
        /// <param name="setting">配置项</param>
        /// <returns>发布是否成功</returns>
        private bool Publish(string queueName, string message, string exchangeName, string exchangeType, string routingKey, bool persistent, QueueSetting setting)
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                if (!string.IsNullOrEmpty(routingKey)) return false;
                exchangeName = "";
                routingKey = queueName;
            }

            //声明新的频道
            var channelDetail = CreateChannelDetail(queueName);
            if (channelDetail is null) return false;
            var channel = channelDetail.Channel;
            if (channel is null) return false;

            bool exclusive = setting.Exclusive;
            bool autoDelete = setting.AutoDelete;
            int expiration = setting.Expiration;
            var arguments = setting.Arguments;

            var dead = setting.DeadLetter;
            this.SetDeadLetter(channel, queueName, ref arguments, dead);
            
            //声明新的队列
            this.CreateQueue(channel, queueName, persistent, exclusive, autoDelete, arguments);

            //声明交换器、并绑定队列
            if (!channelDetail.ExchangeDeclared)
            {
                this.CreateExchange(channel, exchangeName, exchangeType, persistent, autoDelete, arguments);
                this.BindQueue(channel, exchangeName, queueName, routingKey);
                //更新
                channelDetail.ExchangeDeclared = true;
                channelDetail.BindTime = DateTime.Now;
                channelDetail = channelDetails.AddOrUpdate(queueName, channelDetail, (key, oldValue) => {
                    return channelDetail;
                });
            }

            //发布消息
            try
            {
                var msgByte = Encoding.Default.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = persistent;
                if (expiration > 0)
                {
                    properties.Expiration = expiration.ToString();
                }
                channel.BasicPublish(exchangeName, routingKey, properties, msgByte);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetDeadLetter(IModel channel, string queueName, ref IDictionary<string, object> arguments, DeadLetterSetting dead)
        {
            if (dead == null || dead.Expiration < 1) return;

            var deadQueuename = string.Format("{0}.{1}", queueName, dead.DeadQueueExpiration == 0 ? "Dead" : "Live");

            //声明死信交换器
            this.CreateExchange(channel, dead.ExchangeName, MatchType.Direct.ToString(), dead.Persistent, dead.AutoDelete, null);
            
            //死信的死信
            var rguments2 = dead.Arguments;
            if (dead.DeadQueueExpiration > 0)
            {
                if (rguments2 == null || rguments2.Count == 0) rguments2 = new Dictionary<string, object>();
                var deadQueuename2 = string.Format("{0}.Dead", queueName);
                rguments2.Add("x-dead-letter-exchange", dead.ExchangeName);
                rguments2.Add("x-dead-letter-routing-key", deadQueuename2);
                rguments2.Add("x-message-ttl", dead.DeadQueueExpiration);
                //声明死信的死信队列
                this.CreateQueue(channel, deadQueuename2, dead.Persistent, dead.Exclusive, dead.AutoDelete, null);
                //绑定死信交换器和队列
                this.BindQueue(channel, dead.ExchangeName, deadQueuename2, deadQueuename2);
            }
            //声明死信队列
            this.CreateQueue(channel, deadQueuename, dead.Persistent, dead.Exclusive, dead.AutoDelete, rguments2);
            //绑定死信交换器和队列
            this.BindQueue(channel, dead.ExchangeName, deadQueuename, deadQueuename);

            if (arguments == null) arguments = new Dictionary<string, object>();
            //为主队列添加信息设置
            arguments.Add("x-dead-letter-exchange", dead.ExchangeName);
            arguments.Add("x-dead-letter-routing-key", deadQueuename);
            arguments.Add("x-message-ttl", dead.Expiration);
        }

        #endregion

        #region 对外输出方法

        /// <summary>
        /// 发布消息队列
        /// </summary>
        /// <param name="queueName">队列名</param>
        /// <param name="message">队列信息</param>
        /// <param name="setting">设置项</param>
        /// <returns>发布是否成功</returns>
        public bool Publish<T>(string queueName, T message, QueueSetting setting = null)
        {
            if (setting == null) setting = new QueueSetting();

            //消息序列化
            var type = typeof(T).ToString().ToLower();
            string _message;
            if (Constants.BasicTypes.Contains(type))
            {
                _message = message.ToString();
            }
            else
            {
                _message =JsonUtil.ToJson(message);
            }

            return this.Publish(queueName, _message, setting.ExchangeName, setting.MatchType.ToString(), setting.RoutingKey, setting.Persistent, setting);
        }


        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="queueName">队列名</param>
        /// <param name="handler">事件处理方法</param>
        /// <param name="setting">设置项</param>
        public void BeginReceive(string queueName, EventHandler<QueueEventArgs> handler, QueueSetting setting = null)
        {
            if (setting == null) setting = new QueueSetting();

            //声明新的频道
            var channel = CreateChannelDetail(queueName).Channel;
            if (channel is null) return;

            var dead = setting.DeadLetter;
            var arguments = setting.Arguments;
            this.SetDeadLetter(channel, queueName, ref arguments, dead);

            //声明队列
            this.CreateQueue(channel, queueName, setting.Persistent, setting.Exclusive, setting.AutoDelete, arguments);

            //消费者
            var consumer = new EventingBasicConsumer(channel);

            //noAck=true时拿到就删除,channel.BasicQos无效
            //noAck=fasle时需要应答才删除,channel.BasicQos设置最大无应答数，超过不再获得
            if (setting.NoAckCount > 0 && !setting.NoAck)
            {
                channel.BasicQos(0, setting.NoAckCount, false);
            }

            //处理消息
            consumer.Received += (sender, arg) =>
            {
                if (setting.ReceiveDelay > 0)
                {
                    Thread.Sleep(setting.ReceiveDelay);
                }
                handler(sender, new QueueEventArgs(arg.DeliveryTag, arg.Body.ToArray(), setting.Display));
            };
            channel.BasicConsume(queueName, setting.NoAck, consumer);
        }

        /// <summary>
        /// 接收到新消息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">参数</param>
        protected virtual void RaiseNewQueue(object sender, BasicDeliverEventArgs e)
        {
            QueueHanlder?.Invoke(sender, new QueueEventArgs(e.DeliveryTag, e.Body.ToArray()));
        }

        /// <summary>
        /// 应答处理
        /// </summary>
        /// <param name="sender">发起消息的对象</param>
        /// <param name="index">当前消息的序列</param>
        /// <param name="multipleAck">是否批量应答</param>
        /// <param name="ackType">回应类型</param>
        /// <returns>应答是否成功</returns>
        public bool AfterReceive(object sender, ulong index, bool multipleAck = false, AckType ackType = AckType.SuccessAndDelete)
        {
            try
            {
                var channel = ((EventingBasicConsumer)sender).Model;
                switch (ackType)
                {
                    case AckType.SuccessAndDelete:
                        channel.BasicAck(index, multipleAck);
                        break;
                    case AckType.FailButDelete:
                        channel.BasicNack(index, multipleAck, false);
                        break;
                    case AckType.FailButRequeue:
                        channel.BasicNack(index, multipleAck, true);
                        break;
                    case AckType.FailButRequeueAndNewConsumer:
                        channel.BasicRecover(true);
                        break;
                    case AckType.FailButRequeueAndSameConsumer:
                        channel.BasicRecover(false);
                        break;
                    default:
                        channel.BasicAck(index, multipleAck);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 结束释放

        /// <summary>
        ///  终结器释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 终结器释放资源
        /// </summary>
        /// <param name="disposing">清理托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)//清理托管资源
                {
                    
                }
                //清理非托管资源
                DisposeConnection(publishConnection);
                DisposeConnection(readConnection);
                disposedValue = true;
            }
        }
        private void DisposeConnection(IConnection conn)
        {
            if (conn != null && conn.IsOpen)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        #endregion
    }
}
