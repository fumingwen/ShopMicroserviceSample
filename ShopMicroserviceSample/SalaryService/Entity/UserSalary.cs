using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 用户工资档案
    /// </summary>
    public class UserSalary : SalaryBase
    {
        /// <summary>
        /// 用户工资档案Id，自增，添加时不能赋值
        /// </summary>
        public long UserSalaryId { get; set; }

        /// <summary>
        /// 用户Id（非商城）
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 员工类型：0试用，1转正
        /// </summary>
        public int StaffType { get; set; }

        /// <summary>
        /// 生效日期yyyyMMdd
        /// </summary>
        public int FromDate { get; set; }

        /// <summary>
        /// 结束日期yyyyMMdd
        /// </summary>
        public int EndDate { get; set; }

        /// <summary>
        /// 状态：0无效，1有效
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
