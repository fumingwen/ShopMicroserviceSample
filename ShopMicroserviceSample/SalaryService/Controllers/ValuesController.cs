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
    [Authorize("Permission")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IUserService userService;

        public ValuesController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var result = userService.Find(1);
            return Json(result);
        }
    }
}
