using Autofac;
using Common.Caches;
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
            //��ȡ�����ļ�
            var audienceConfig = Configuration.GetSection("Audience");
            services.AddOcelotPolicyJwtBearer(audienceConfig["Issuer"], audienceConfig["Audience"], audienceConfig["Secret"], "GSWBearer", "Permission", "no permission");
            //�������ģ���û�Ȩ�ޱ�,�ɴ����ݿ��в�ѯ����
            var permission = new List<Permission> {
                              new Permission {  Url="/", Name="system"},
                              new Permission {  Url="/api/values", Name="system"}
                          };
            services.AddSingleton(permission);
            services.AddControllers();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //ע��ExceptionlessLogger����
            services.AddSingleton<ILoggerHelper, ExceptionlessLogger>();

            //ע��Configuration
            services.AddSingleton(Configuration); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ExceptionlessClient.Default.Configuration.ApiKey = Configuration.GetSection("Exceptionless:ApiKey").Value;
            ExceptionlessClient.Default.Configuration.ServerUrl = Configuration.GetSection("Exceptionless:ServerUrl").Value;
            app.UseExceptionless();

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
