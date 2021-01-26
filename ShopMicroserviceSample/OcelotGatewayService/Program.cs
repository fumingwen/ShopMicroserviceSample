using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OcelotGatewayService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("OcelotGatewayService 服务已启动，端口号为：6800，时间：{0}", DateTime.Now);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls("http://*:6800")   //设置默认访问端口
                        .UseStartup<Startup>();
                    });
    }
}
