using Common.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaryService.Entity;
using SalaryService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UserController : Controller
    { 
        private readonly IUserService userService; 

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: api/users  
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            var result = userService.SearchRequest(new SearchRequest<UserModel>(new UserModel() { RealName = "姓名" }));
            return result.Data;
        }
    }
}
