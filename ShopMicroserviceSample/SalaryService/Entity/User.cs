using Common.Database;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Entity
{
    /// <summary>
    /// 内部用户
    /// </summary>
    public class User : EntityBase<long>
    {
        /// <summary>
        /// 用户Id，自增，添加时不能赋值
        /// </summary>
        [FiledComment(IsIdentity = true)]
        public long UserId { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        public long UserNo { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 部门名称，编辑时不用传
        /// </summary>
        public String DepartmentName { get; set; }

        /// <summary>
        /// 系列Id
        /// </summary>
        public int SubDepartmentId { get; set; }

        /// <summary>
        /// 系列名称，编辑时不用传
        /// </summary>
        public String SubDepartmentName { get; set; }

        /// <summary>
        /// 团队Id
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// 团队名称，编辑时不用传
        /// </summary>
        public String TeamName { get; set; }

        /// <summary>
        /// 岗位Id
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// 岗位名称，编辑时不用传
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// 亲情扣款，工资发给亲人
        /// </summary>
        public decimal FamilyDeduction { get; set; }

        /// <summary>
        /// 员工类型：0试用，1转正，2临时工，88辞职
        /// </summary>
        public int StaffType { get; set; }

        /// <summary>
        /// 入职日期yyyyMMdd
        /// </summary>
        public int SignDate { get; set; }

        /// <summary>
        /// 入职日期yyyyMMdd
        /// </summary>
        public int RegularWorkerDate { get; set; }

        /// <summary>
        /// 辞职日期yyyyMMdd，工作包括当天
        /// </summary>
        public int ResignDate { get; set; }

        /// <summary>
        /// 状态：0待审核，1正常，2临时冻结，4屏蔽
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 旧系统用户自增Id
        /// </summary>
        public long OldUserId { get; set; }

        /// <summary>
        /// 用户薪资体系列表，编辑时不用传
        /// </summary>
        [FiledComment(Ignore = true)]
        public List<UserSalary> UserSalaries { get; set; }

        /// <summary>
        /// 岗位薪资体系，编辑时不用传
        /// </summary>
        [FiledComment(Ignore = true)]
        public PositionSalary PositionSalary { get; set; }
    }

    public static class UserExtends
    {
        private static Dictionary<string, int> statuses = new Dictionary<string, int> { { "待审核", 0 }, { "正常", 1 }, { "临时冻结", 2 }, { "屏蔽", 4 }, { "停发工资", 88 } };
        private static Dictionary<string, int> staffTypes = new Dictionary<string, int> { { "试用", 0 }, { "转正", 1 }, { "临时工", 2 }, { "辞职", 88 } };

        public static List<T> SetPassword<T>(this List<T> users) where T : User
        {
            foreach (var u in users)
            {
                var password = u.Password;
                if (!string.IsNullOrEmpty(password)) password = EncryptUtil.TransDecrypt(u.Password);
                u.Password = EncryptUtil.Md5(password);
            }
            return users;
        }
        public static List<UserTemp> SetDate(this List<UserTemp> users)
        {
            foreach (var u in users)
            {
                if (u.SignDateTime == null || u.SignDateTime < new DateTime(1900, 1, 1)) u.SignDateTime = new DateTime(2019, 1, 1);
                if (u.RegularWorkerDateTime == null || u.RegularWorkerDateTime < new DateTime(1900, 1, 1)) u.RegularWorkerDateTime = new DateTime(2099, 12, 31);
                if (u.ResignDateTime == null || u.ResignDateTime < new DateTime(1900, 1, 1)) u.ResignDateTime = new DateTime(2099, 12, 31);

                u.SignDate = u.SignDateTime.GetShortDateInteger();
                u.RegularWorkerDate = u.RegularWorkerDateTime.GetShortDateInteger();
                u.ResignDate = u.ResignDateTime.GetShortDateInteger();
            }

            return users;
        }

        public static List<UserTemp> SetStatus(this List<UserTemp> users)
        {
            foreach (var u in users)
            {
                u.StaffType = staffTypes[u.StaffTypeString];
                u.Status = statuses[u.StatusString];
            }
            return users;
        }
    }
}
