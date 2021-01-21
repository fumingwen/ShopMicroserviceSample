using SalaryService.Entity;
using ServiceBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Service
{
    public interface IDepartmentService : IServiceBasic<Department, int, DepartmentModel>
    {
        //自行扩展或重写 
    }
}
