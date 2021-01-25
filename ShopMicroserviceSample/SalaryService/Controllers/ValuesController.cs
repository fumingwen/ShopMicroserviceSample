using Common.Log;
using Common.MongoDB;
using Common.Queues;
using DotNetCore.CAP;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalaryService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Controllers
{
    [Authorize("Permission")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDepartmentService departmentService;
        private readonly IUserService userService;
        private readonly ILoggerHelper loggerHelper;

        public ValuesController(IUserService userService, IDepartmentService departmentService, ILoggerHelper loggerHelper)
        {
            this.departmentService = departmentService;
            this.userService = userService;
            this.loggerHelper = loggerHelper;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {

            var helper = new MongoDbHelper("EdayingDB", false, true);
            var result = userService.Find(2);


            #region Exceptionless测试

            loggerHelper.Debug("Salary", JsonConvert.SerializeObject(result.Data));
            loggerHelper.Error("Salary", JsonConvert.SerializeObject(result.Data));
            loggerHelper.Info("Salary", JsonConvert.SerializeObject(result.Data));
            loggerHelper.Trace("Salary", JsonConvert.SerializeObject(result.Data));
            loggerHelper.Warn("Salary", JsonConvert.SerializeObject(result.Data));

            #endregion

            Task.Run(() => helper.Insert("User", result.Data));

            return Json(result);
        }
    }
}
