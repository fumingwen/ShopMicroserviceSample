using DataBasic;
using Microsoft.Extensions.Configuration;
using SalaryService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Data
{
    public class DepartmentData : DataBasic<Department, int, DepartmentModel>, IDepartmentData
    {
        public DepartmentData(IConfiguration configuration) : base(configuration)
        {
            base.TableMapper = new TableMapper<Department>()
            {
                TableName = "Department",
                ShortTableName = "de",
                PrimaryKeys = "[DepartmentId]={0}",
                OriginalFileds = new List<string>() { "DepartmentId", "DepartmentName" },
                InsertIgnoreFileds = new List<string>() { "DepartmentId" },
                InsertWhere = "",
                UpdateIgnoreFileds = new List<string>() { "DepartmentId" },
                UpdateWhere = "[DepartmentId]=@DepartmentId and not exists(select 1 from Department d where d.DepartmentId <> @DepartmentId and d.DepartmentName=@DepartmentName)",
                DefaultOrderBy = "[DepartmentId]"
            };
        }

        //自行扩展或重写
    }
}
