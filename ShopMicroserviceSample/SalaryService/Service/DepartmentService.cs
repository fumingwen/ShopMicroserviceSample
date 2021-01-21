using Common.Caches;
using Common.Database;
using SalaryService.Data;
using SalaryService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Service
{
    public class DepartmentService : ServiceBasic.ServiceBasic<Department, int, DepartmentModel, IDepartmentData>, IDepartmentService
    {
        private readonly IDepartmentData departmentData; 
        private readonly ICache cache;

        public DepartmentService(IDepartmentData departmentData, ICache cache) : base(departmentData, cache)
        {
            this.departmentData = departmentData; 
            this.cache = cache;
        }

        //自行扩展或重写 
    }
}
