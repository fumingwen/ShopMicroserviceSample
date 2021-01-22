using Common.Queue;
using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQService
{
    public class MQListener
    {
        private readonly ICapPublisher capPublisher;
        public MQListener(ICapPublisher capPublisher)
        {
            this.capPublisher = capPublisher;
        }

        public async Task Listener()
        {
            var configurationRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var queueName = configurationRoot["RabbitMQ:QueueName"];
            IQueue queue1 = new RabbitQueue();

            Console.WriteLine("接收MQ消息中......");

            while (true)
            {
                queue1.BeginReceive(queueName, (sender, queue) =>
                {
                    Console.WriteLine("接收到队列[{0}]的消息，内容;[{1}]", queueName, queue.Message);
                    capPublisher.Publish(queueName, queue.Message);
                    queue1.AfterReceive(sender, queue.Index, false);    
                });
            }

        }
    }
}
