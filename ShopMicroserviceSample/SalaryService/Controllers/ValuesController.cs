using Common.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalaryService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Controllers
{
    //[Authorize("Permission")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDepartmentService departmentService;
        private readonly IUserService userService;

        public ValuesController(IUserService userService, IDepartmentService departmentService)
        {
            this.departmentService = departmentService;
            this.userService = userService;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var helper = new MongoDbHelper("EdayingDB", false, true);
            var result = userService.Find(1);

            Task.Run(() => helper.Insert("User", result.Data));

            return Json(result);
        }
    }
}
