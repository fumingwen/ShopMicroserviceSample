using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OcelotGatewayService.Controllers
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
            return Content("OcelotGatewayService KeyHealth检查成功！");
        }
    }
}
