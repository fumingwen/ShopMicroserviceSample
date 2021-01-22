using App.Metrics;
using JWTAuthorizePolicy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcelotGatewayService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region Metrics�������
            string IsOpen = Configuration.GetSection("InfluxDB:IsOpen").Value.ToLower();
            if (IsOpen == "true")
            {
                string database = Configuration.GetSection("InfluxDB")["DataBaseName"];
                string InfluxDBConStr = Configuration.GetSection("InfluxDB")["ConnectionString"];
                string app = Configuration.GetSection("InfluxDB")["app"];
                string env = Configuration.GetSection("InfluxDB")["env"];
                string username = Configuration.GetSection("InfluxDB")["username"];
                string password = Configuration.GetSection("InfluxDB")["password"];
                var uri = new Uri(InfluxDBConStr);

                var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(
                options =>
                {
                    options.AddAppTag(app);
                    options.AddEnvTag(env);
                })
                .Report.ToInfluxDb(
                options =>
                {
                    options.InfluxDb.BaseUri = uri;
                    options.InfluxDb.Database = database;
                    options.InfluxDb.UserName = username;
                    options.InfluxDb.Password = password;
                    options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                    options.HttpPolicy.FailuresBeforeBackoff = 5;
                    options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                    options.FlushInterval = TimeSpan.FromSeconds(5);
                })
                .Build();

                services.AddMetrics(metrics);
                services.AddMetricsReportScheduler();
                services.AddMetricsTrackingMiddleware();
                services.AddMetricsEndpoints();
            }

            #endregion

            var audienceConfig = Configuration.GetSection("Audience");
            //ע��OcelotJwtBearer
            services.AddOcelotJwtBearer(audienceConfig["Issuer"], audienceConfig["Issuer"], audienceConfig["Secret"], "FMWBearer");

            //���Ocelot��ע��configuration.json��·�����ұ���ͷ����˸�·����
            services.AddOcelot(new ConfigurationBuilder()
                .AddJsonFile("configuration.json", true, true).Build())
                .AddPolly() //��� Ocelot.Provider.Polly ʵ���۶�
                .AddCacheManager(x => x.WithDictionaryHandle());  // ��� Ocelot.Cache.CacheManager ʵ�ֻ���

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region ʹ���м��Metrics
            string IsOpen = Configuration.GetSection("InfluxDB")["IsOpen"].ToLower();
            if (IsOpen == "true")
            {
                app.UseMetricsAllMiddleware();
                app.UseMetricsAllEndpoints();
            }
            #endregion

            //����ʹ��Ocelot
            app.UseOcelot().Wait();
        } 
    }
}
