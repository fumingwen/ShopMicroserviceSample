using Common.Queue;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;  

namespace MQService
{
    class Program
    {
          
        static void Main(string[] args)
        { 
            var configurationRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var queueName = configurationRoot["RabbitMQ:QueueName"]; 
            IQueue queue1 = new RabbitQueue();

            Console.WriteLine("接收MQ消息中......");

            while (true)
            {
                queue1.BeginReceive(queueName, (sender, queue) =>
                {
                    //receive a message
                    var message = "我收到消息啦！" + queue.Message;
                    Console.WriteLine(message);
                    queue1.AfterReceive(sender, queue.Index, false);    //delete it after respond 
                });
            }

        }
    }
}
