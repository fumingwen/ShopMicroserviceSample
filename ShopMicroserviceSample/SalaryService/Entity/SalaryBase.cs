using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 用户工资档案
    /// </summary>
    public abstract class SalaryBase : EntityBase<long>
    {
        /// <summary>
        /// 基本工资
        /// </summary>
        public decimal BasicSalary { get; set; }

        /// <summary>
        /// 岗位补贴
        /// </summary>
        public decimal PositionSubsidy { get; set; }

        /// <summary>
        /// 社保补贴
        /// </summary>
        public decimal SocialSecuritySubsidy { get; set; }

        /// <summary>
        /// 公积金补贴
        /// </summary>
        public decimal AccumulationFundSubsidy { get; set; }

        /// <summary>
        /// 亲情补贴
        /// </summary>
        public decimal FamilySubsidy { get; set; }

        /// <summary>
        /// 额外补贴
        /// </summary>
        public decimal AdditionalSubsidy { get; set; }

        /// <summary>
        /// 其他补贴
        /// </summary>
        public decimal OtherSubsidy { get; set; }

        /// <summary>
        /// 绩效考核奖金
        /// </summary>
        public decimal AchievementsReward { get; set; }

        /// <summary>
        /// 全勤奖
        /// </summary>
        public decimal FullAttendanceReward { get; set; }

        /// <summary>
        /// 激励政策
        /// </summary>
        public decimal IncentivePolicyReward { get; set; }
    }
}
