using Common.Queue; 
using Exceptionless;
using Exceptionless.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;  

namespace MQService
{
    class Program
    {
          
        static void Main(string[] args)
        {
            ExceptionlessClient.Default.Configuration.ApiKey = "mFUwHaX47rWy30AGexwltm0rf504zgend1i9zZge";
            ExceptionlessClient.Default.Configuration.ServerUrl = "http://localhost:50000";
            ExceptionlessClient.Default.Startup();
            ExceptionlessClient.Default.SubmitLog("这是一个普通日志记录code:{12345678999}", LogLevel.Info);

            try
            {
                ExceptionlessClient.Default.CreateLog("出错了", LogLevel.Error).Submit(); ;
                throw new Exception($"看这里异常了!时间：{DateTime.Now}");
            }
            catch(Exception ex)
            {
                ex.ToExceptionless().Submit();
            }

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
