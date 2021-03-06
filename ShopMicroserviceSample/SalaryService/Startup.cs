using Autofac;
using Common.Caches;
using Common.Consul;
using Common.Log;
using Exceptionless;
using JWTAuthorizePolicy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SalaryService.Data;
using SalaryService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            services.AddOcelotPolicyJwtBearer(audienceConfig["Issuer"], audienceConfig["Audience"], audienceConfig["Secret"], "GSWBearer", "Permission", "no permission");
            //这个集合模拟用户权限表,可从数据库中查询出来
            var permission = new List<Permission> {
                              new Permission {  Url="/", Name="system"},
                              new Permission {  Url="/api/values", Name="system"}
                          };
            services.AddSingleton(permission);
            services.AddControllers();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //注入ExceptionlessLogger服务
            services.AddSingleton<ILoggerHelper, ExceptionlessLogger>();

            //注入Configuration
            services.AddSingleton(Configuration);

            services.AddCap(x =>
            {
                x.Version = Configuration["RabbitMQ:ExchangeName"];
                //x.ConsumerThreadCount = 15;
                x.UseMongoDB(op =>
                {
                    op.DatabaseConnection = Configuration["MongoDB"];
                    op.DatabaseName = "EdayingDB";
                    op.PublishedCollection = "EdayingPublish";
                    op.ReceivedCollection = "EdayingReceived";
                });
                x.UseRabbitMQ(o =>
                { 
                    o.HostName = Configuration["RabbitMQ:HostName"];
                    o.UserName = Configuration["RabbitMQ:UserName"];
                    o.Password = Configuration["RabbitMQ:Password"];
                    o.Port = int.Parse(Configuration["RabbitMQ:Port"]);
                });
                x.ConsumerThreadCount = 3;
                x.FailedRetryCount = 0;
                x.FailedRetryInterval = 10;
                x.UseDashboard();
                x.Version = Configuration["RabbitMQ:ExchangeName"]; 
            });

            services.AddSingleton(Configuration.GetSection("Consul").Get<ConsulOption>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ConsulOption consulOption)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ExceptionlessClient.Default.Configuration.ApiKey = Configuration.GetSection("Exceptionless:ApiKey").Value;
            ExceptionlessClient.Default.Configuration.ServerUrl = Configuration.GetSection("Exceptionless:ServerUrl").Value;
            app.UseExceptionless();

            // 注册Consul
            app.RegisterConsul(lifetime, consulOption);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<ConfigureAutofac>();
        }
    }
}
