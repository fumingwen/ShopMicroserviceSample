using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 定岗工资
    /// </summary>
    public class PositionSalary : SalaryBase
    {
        /// <summary>
        /// 定岗工资自增Id
        /// </summary>
        [FiledComment(IsIdentity = true)]
        public int PositionSalaryId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [FiledComment(TableName = "de")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 岗位Id
        /// </summary>
        [FiledComment(TableName = "po")]
        public int PositionId { get; set; }

        /// <summary>
        /// 人才培养工资
        /// </summary>
        public decimal PersonnelTraining { get; set; }

        /// <summary>
        /// 目标达成奖
        /// </summary>
        public decimal ObjectivesAward { get; set; }

        /// <summary>
        /// 冲刺目标达成奖
        /// </summary>
        public decimal SprintAward { get; set; }

        /// <summary>
        /// 季度奖金
        /// </summary>
        public decimal QuarterlyReward { get; set; }

        /// <summary>
        /// 团队PK奖励
        /// </summary>
        public decimal TeamPK { get; set; }

        /// <summary>
        /// 个人提成配置Json
        /// </summary>
        public String PersonalCommissionConfig { get; set; }

        /// <summary>
        /// 团队提成配置Json
        /// </summary>
        public String TeamCommissionConfig { get; set; }

        /// <summary>
        /// 系列提成配置
        /// </summary>
        public String SubDepartmentCommissionConfig { get; set; }

        /// <summary>
        /// 部门提成配置
        /// </summary>
        public String DepartmentCommissionConfig { get; set; }

    }
}
