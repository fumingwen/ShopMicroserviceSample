using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 部门信息搜索参数
    /// </summary>
    public class DepartmentModel
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public String DepartmentName { get; set; }

        /// <summary>
        /// 是否加载系列
        /// </summary>
        [FiledComment(false)]
        public bool? WithSubDepartment { get; set; }

    }
}
