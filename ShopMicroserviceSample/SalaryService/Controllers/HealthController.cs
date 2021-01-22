using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Queues;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SalaryService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class HealthController : Controller
    { 
        /// <summary>
        /// 主健康检查
        /// </summary>
        /// <returns></returns>
        [HttpGet("/health")]
        public IActionResult KeyHealth()
        {  
            return  Content("SalaryService KeyHealth检查成功！");
        }

        [CapSubscribe("Salary")]
        public IActionResult Exce(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            { 
                Console.WriteLine("This Is Exce:{0}", name);
            }

            return Json(default);
        }
    }
}
