using Common.Caches;
using Common.Log;
using Exceptionless; 
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQService
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
            services.AddCap(x =>
            {
                x.Version = Configuration["RabbitMQ:ExchangeName"]; 
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 

            app.UseRouting();  
        }
    }
}
