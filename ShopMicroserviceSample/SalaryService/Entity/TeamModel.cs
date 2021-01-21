using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 团队搜索参数
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// 团队Id
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// 团队Ids
        /// </summary>
        public List<int> TeamIds { get; set; }

        /// <summary>
        /// 系列Id
        /// </summary>
        [FiledComment(TableName = "te")]
        public int? SubDepartmentId { get; set; }

        /// <summary>
        /// 系列Ids
        /// </summary>
        public List<int> SubDepartmentIds { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [FiledComment(TableName = "te")]
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 部门Ids
        /// </summary>
        public List<int> DepartmentIds { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public String TeamName { get; set; }

    }
}
