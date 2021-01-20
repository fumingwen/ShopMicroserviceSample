using DataBasic;
using Microsoft.Extensions.Configuration;
using SalaryService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryService.Data
{
    public class UserData : DataBasic<User, long, UserModel>, IUserData
    {
        private readonly IConfiguration configuration;
        public UserData(IConfiguration configuration) : base(configuration)
        { 
            base.TableMapper = new TableMapper<User>()
            {
                TableName = "User",
                ShortTableName = "us",
                PrimaryKeys = "[UserId]={0}",
                OriginalFileds = new List<string>() { "UserId", "UserNo", "RealName", "Password", "DepartmentId", "DepartmentName", "SubDepartmentId", "SubDepartmentName", "TeamId", "TeamName", "PositionId", "PositionName", "StaffType", "RegularWorkerDate", "SignDate", "FamilyDeduction", "ResignDate", "Status", "OldUserId" },
                InsertIgnoreFileds = new List<string>() { "UserId" },
                InsertWhere = "",
                UpdateIgnoreFileds = new List<string>() { "UserId" },
                UpdateWhere = "[UserId]=@UserId",
                DefaultOrderBy = "[UserId]"
            };
        }

        //自行扩展或重写
        public User FindByUserNo(long userNo)
        {
            var sql = string.Format("select {0} from {1} where [UserNo]={2}", TableMapper.SelectFileds, TableMapper.TableName, userNo);
            return this.Query<User>(sql);
        }
        public int UpdatePositionName(Position position)
        {
            return this.Execute($"UPDATE [USER] SET PositionName=@PositionName WHERE PositionId=@PositionId", new { PositionName = position.PositionName, PositionId = position.PositionId });
        }

        public int UpdateDepartment(Department department)
        {
            var sql = $"update [User] set DepartmentName='{department.DepartmentName}' where DepartmentId={department.DepartmentId}";
            return this.Execute(sql);
        }

        public int UpdateSubDepartment(SubDepartment subDepartment)
        {
            var sql = $"update [User] set SubDepartmentName='{subDepartment.SubDepartmentName}' where SubDepartmentId={subDepartment.SubDepartmentId}";
            return this.Execute(sql);
        }

        public int UpdateTeam(Team team)
        {
            var sql = $"update [User] set TeamName='{team.TeamName}' where TeamId={team.TeamId}";
            return this.Execute(sql);
        }

        public List<User> SearchByRealNameDepartmentAndUserNo(List<User> users)
        {
            var sb = new StringBuilder();
            sb.Append("select u.* from [User] u inner join (");
            foreach (var u in users)
            {
                sb.AppendFormat("select {0} as UserNo, '{1}' as RealName, '{2}' as DepartmentName union ", u.UserNo, u.RealName, u.DepartmentName);
            }
            sb.Append(")t").Replace(" union )t", ")t");
            sb.Append(" on u.RealName=t.RealName and u.DepartmentName=t.DepartmentName and (t.UserNo<1 or u.UserNo=t.UserNo)");

            return this.Querys<User>(sb.ToString());
        }
    }
}
