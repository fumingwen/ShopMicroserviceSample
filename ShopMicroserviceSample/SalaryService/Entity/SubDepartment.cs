using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 系列
    /// </summary>
    public class SubDepartment : EntityBase<int>
    {
        /// <summary>
        /// 系列Id，自增，添加时不能赋值
        /// </summary>
        [FiledComment(IsIdentity = true)]
        public int SubDepartmentId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [FiledComment(UpdateIgnore = true)]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 系列名称
        /// </summary>
        public string SubDepartmentName { get; set; }

        /// <summary>
        /// 团队列表
        /// </summary>
        public List<Team> Teams { get; set; }

    }
}
