using System;
using System.Threading;

namespace Common.Queue
{

    /// <summary>
    /// RabbitMQ 测试看看
    /// </summary>
    class ForTest
    {
        /// <summary>
        /// Send a message
        /// </summary>
        public void Publish()
        {
            //default server
            IQueue iqueue = new RabbitQueue();

            //message to be sent can be any type
            var message = new Message() { Time = DateTime.Now, Index = 1, Detail = "您好" };

            //send a string message with default setting
            iqueue.Publish("queueNameForString", message.Detail);

            //send a class object message with default setting
            iqueue.Publish("queueNameForClass", message);

            //send a string message with advanced setting
            iqueue.Publish("queueNameForStringWithSetting", message.Detail,
                new QueueSetting()
                {
                    Persistent = false,             //whether to save the message to the disk to keep it for a long time
                    ExchangeName = "newExchange",   //declare an exchange
                    RoutingKey = "newRouting",      //decalre a routing key
                    Expiration = 600000             //setting the time period during which the message is valid
                });

            //send a class object message with advanced setting
            iqueue.Publish("queueNameForClassWithSetting", message,
                new QueueSetting()
                {
                    Persistent = false,                 //whether to save the message to the disk to keep it for a long time
                    ExchangeName = "newExchangeClass",  //declare an exchange
                    RoutingKey = "newRoutingClass",     //decalre a routing key
                    Expiration = 600000                 //setting the time period during which the message is valid
                });

            //end now
            iqueue.Dispose();
        }

        /// <summary>
        /// Receive a message
        /// setting must be the same as publishing 
        /// </summary>
        public void Receive()
        {
            //default server
            IQueue iqueue = new RabbitQueue();

            //receive a string message with default setting
            iqueue.BeginReceive("queueNameForString", (sender, queue) =>
            {
                //receive a message
                var message = "我收到消息啦！" + queue.Message;
                iqueue.AfterReceive(sender, queue.Index, false);    //delete it after respond 
            });

            //receive a class object message with default setting
            iqueue.BeginReceive("queueNameForClass", (sender, queue) =>
            {
                //receive a message
                var message = queue.ToObject<Message>();
                iqueue.AfterReceive(sender, queue.Index, false);    //delete it after respond 
            });

            //receive a string message with advanced setting
            iqueue.BeginReceive("queueNameForStringWithSetting", (sender, queue) =>
            {
                //receive a message
                var message = "我收到消息啦！" + queue.Message;
                iqueue.AfterReceive(sender, queue.Index, false);    //delete it after respond 
            }, new QueueSetting()
            {
                Persistent = false,             //error if not the same as publishing
                RoutingKey = "newRouting"      //fail to receive any message if not the same as publishing
            });

            //receive a class object message with advanced setting
            iqueue.BeginReceive("queueNameForClassWithSetting", (sender, queue) =>
            {
                //receive a message
                var message = queue.ToObject<Message>();
                iqueue.AfterReceive(sender, queue.Index, false);    //delete it after respond
            }, new QueueSetting()
            {
                Persistent = false,             //error if not the same as publishing
                RoutingKey = "newRoutingClass"  //fail to receive any message if not the same as publishing
            });

            Thread.Sleep(50000); //keep waiting before receiving all messages

            //end now
            iqueue.Dispose();
        }
    }

    /// <summary>
    /// 消息类
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time
        {
            get; set;
        }

        /// <summary>
        /// 序列
        /// </summary>
        public int Index
        {
            get; set;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Detail
        {
            get; set;
        }
    }
}
