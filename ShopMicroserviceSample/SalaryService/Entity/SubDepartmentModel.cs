using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 系列搜索参数
    /// </summary>
    public class SubDepartmentModel
    {
        /// <summary>
        /// 系列Id
        /// </summary>
        public int? SubDepartmentId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 部门Ids
        /// </summary>
        public List<int> DepartmentIds { get; set; }

        /// <summary>
        /// 系列名称
        /// </summary>
        public String SubDepartmentName { get; set; }

        /// <summary>
        /// 是否加载团队
        /// </summary>
        [FiledComment(false)]
        public bool? WithTeam { get; set; }

    }
}
