using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 内部用户搜索参数
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public List<long> UserIds { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        public long? UserNo { get; set; }

        /// <summary>
        /// 用户工号s
        /// </summary>
        public List<long> UserNos { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public String RealName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public List<string> RealNames { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public String DepartmentName { get; set; }

        /// <summary>
        /// 系列Id
        /// </summary>
        public int? SubDepartmentId { get; set; }


        /// <summary>
        /// 系列名称
        /// </summary>
        public String SubDepartmentName { get; set; }

        /// <summary>
        /// 团队Id
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public String TeamName { get; set; }

        /// <summary>
        /// 岗位Id
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public String PositionName { get; set; }

        /// <summary>
        /// 员工类型：0试用，1转正，2临时工，88辞职
        /// </summary>
        public int? StaffType { get; set; }

        /// <summary>
        /// 员工类型：0试用，1转正，2临时工，88辞职多选
        /// </summary>
        public List<int> StaffTypes { get; set; }

        /// <summary>
        /// 开始入职日期yyyyMMdd
        /// </summary>
        public int? MinSignDate { get; set; }

        /// <summary>
        /// 截止入职日期yyyyMMdd
        /// </summary>
        public int? MaxSignDate { get; set; }

        /// <summary>
        /// 开始辞职日期yyyyMMdd，工作包括当天
        /// </summary>
        public int? MinResignDate { get; set; }

        /// <summary>
        /// 截止辞职日期yyyyMMdd，工作包括当天
        /// </summary>
        public int? MaxResignDate { get; set; }

        /// <summary>
        /// 状态：0待审核，1正常，2临时冻结，4屏蔽
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 状态：0待审核，1正常，2临时冻结，4屏蔽不等于
        /// </summary>
        public int? NotEqualStatus { get; set; }

        /// <summary>
        /// 状态：0待审核，1正常，2临时冻结，4屏蔽多选
        /// </summary>
        public List<int> Statuses { get; set; }

        /// <summary>
        /// 旧系统用户自增Id
        /// </summary>
        public long? OldUserId { get; set; }

        /// <summary>
        /// 是否加载员工薪资体系
        /// </summary>
        [FiledComment(false)]
        public bool? WithUserSalary { get; set; }

        /// <summary>
        /// 员工薪资体系是否有效
        /// </summary>
        [FiledComment(false)]
        public bool? UserSalaryValid { get; set; }

        /// <summary>
        /// 是否加载定岗薪资体系
        /// </summary>
        [FiledComment(false)]
        public bool? WithPositionSalary { get; set; }
    }
}
