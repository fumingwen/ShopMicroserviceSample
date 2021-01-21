using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    public class TeamExtendsSubDepartmentModel : TeamModel
    {

        /// <summary>
        /// 系列名称
        /// </summary>
        public string SubDepartmentName { get; set; }
    }
}
