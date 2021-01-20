using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 岗位信息
    /// </summary>
    public class Position : EntityBase<int>
    {
        /// <summary>
        /// 岗位Id，自增，添加时不能赋值
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// 旧商城岗位Id
        /// </summary>
        public int OldPositionId { get; set; }

    }
}
