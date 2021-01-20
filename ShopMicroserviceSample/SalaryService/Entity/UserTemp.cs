using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 内部用户临时表
    /// </summary>
    public class UserTemp : User
    {
        /// <summary>
        /// 状态字符串
        /// </summary>
        [FiledComment(Ignore = true)]
        public String StaffTypeString { get; set; }

        /// <summary>
        /// 状态字符串
        /// </summary>
        [FiledComment(Ignore = true)]
        public String StatusString { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        [FiledComment(Ignore = true)]
        public DateTime SignDateTime { get; set; }

        /// <summary>
        /// 转正日期
        /// </summary>
        [FiledComment(Ignore = true)]
        public DateTime RegularWorkerDateTime { get; set; }

        /// <summary>
        /// 辞职日期，工作包括当天
        /// </summary>
        [FiledComment(Ignore = true)]
        public DateTime ResignDateTime { get; set; }
    }
}
