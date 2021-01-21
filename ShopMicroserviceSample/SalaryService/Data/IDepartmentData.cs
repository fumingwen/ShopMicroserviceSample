using DataBasic;
using SalaryService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Data
{
    public interface IDepartmentData : IDataBasic<Department, int, DepartmentModel>
    {
        //自行扩展或重写
    }
}
