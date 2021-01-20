using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 部门信息
    /// </summary>
    public class Department : EntityBase<int>
    {
        /// <summary>
        /// 部门Id，自增，添加时不能赋值
        /// </summary>
        [FiledComment(IsIdentity = true)]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 系列列表
        /// </summary>
        [FiledComment(Ignore = true)]
        public List<SubDepartment> SubDepartments { get; set; }

    }
}
