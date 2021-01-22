using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Queues
{
    /// <summary>
    /// 获取IOC服务
    /// </summary>
    public class ServiceLocator
    {
        private static IServiceProvider Services { get; set; }
        /// <summary>
        /// 设置 IServiceProvider
        ///Program.cs
        ///    var host = CreateHostBuilder(args).Build();
        ///    ServiceLocator.SetServices(host.Services);
        ///    host.Run();
        /// </summary>
        /// <param name="services"></param>
        public static void SetServices(IServiceProvider services)
        {
            Services = services;
        }
        /// <summary>
        /// 获取IOC容器中注册的服务,若服务未注册则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if (Services == null)
            {
                throw new Exception("IServiceProvider is null");
            }
            return (T)Services?.GetService(typeof(T));
        }
    }
}
