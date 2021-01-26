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
            Console.WriteLine("OcelotGatewayService �������������˿ں�Ϊ��6800��ʱ�䣺{0}", DateTime.Now);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls("http://*:6800")   //����Ĭ�Ϸ��ʶ˿�
                        .UseStartup<Startup>();
                    });
    }
}
