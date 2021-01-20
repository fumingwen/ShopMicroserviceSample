using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 团队
    /// </summary>
    public class Team : EntityBase<int>
    {
        /// <summary>
        /// 团队Id，自增，添加时不能赋值
        /// </summary>
        [FiledComment(IsIdentity = true)]
        public int TeamId { get; set; }

        /// <summary>
        /// 系列Id
        /// </summary>
        [FiledComment(UpdateIgnore = true, TableName = "te")]
        public int SubDepartmentId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [FiledComment(UpdateIgnore = true, TableName = "te")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 提成系数
        /// </summary>
        public decimal SaleRatix { get; set; }

    }
}
