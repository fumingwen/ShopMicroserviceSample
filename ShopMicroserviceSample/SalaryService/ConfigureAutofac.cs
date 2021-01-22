using Autofac;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SalaryService
{
    public class ConfigureAutofac : Autofac.Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            Assembly Repository = Assembly.Load("Common");
            Assembly IRepository = Assembly.Load("Common");
            containerBuilder.RegisterAssemblyTypes(Repository, IRepository)
.Where(t => t.Name.ExistsIn("Redis", "NPOIExcel", "RabbitQueue"))
.AsImplementedInterfaces().PropertiesAutowired();

            Assembly service = Assembly.Load("SalaryService");
            Assembly Iservice = Assembly.Load("SalaryService");
            containerBuilder.RegisterAssemblyTypes(service, Iservice)
.Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Data"))
.AsImplementedInterfaces().PropertiesAutowired();
             
            #region 在控制器中使用属性依赖注入，其中注入属性必须标注为public 

            var controllersTypesInAssembly = typeof(Startup).Assembly.GetExportedTypes()
.Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type)).ToArray();
            containerBuilder.RegisterTypes(controllersTypesInAssembly).PropertiesAutowired();

            #endregion
        }
    }
}
