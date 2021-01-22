using Common.Queue;
using Common.Queues;
using DotNetCore.CAP;
using Exceptionless;
using Exceptionless.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;  

namespace MQService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            ServiceLocator.SetServices(host.Services);
            var capBus = ServiceLocator.GetService<ICapPublisher>();

            var configurationRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var queueName = configurationRoot["RabbitMQ:QueueName"];
            IQueue queue1 = new RabbitQueue();

            Console.WriteLine("接收MQ消息中......");

            while (true)
            {
                queue1.BeginReceive(queueName, (sender, queue) =>
                {
                    Console.WriteLine("接收到队列[{0}]的消息，内容;[{1}]", queueName, queue.Message);
                    capBus.Publish(queueName, queue.Message);
                    queue1.AfterReceive(sender, queue.Index, false);
                });
            } 
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                 Host.CreateDefaultBuilder(args)
                     .ConfigureWebHostDefaults(webBuilder =>
                     {
                         webBuilder.UseUrls("http://*:6810");
                         webBuilder.UseStartup<Startup>();
                     });
    }
}
