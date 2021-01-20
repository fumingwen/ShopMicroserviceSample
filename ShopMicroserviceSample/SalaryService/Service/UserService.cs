using Common;
using Common.Caches;
using Common.Database;
using Common.Tools;
using SalaryService.Data;
using SalaryService.Entity;
using ServiceBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryService.Service
{
    public class UserService : ServiceBasic<User, long, UserModel, IUserData>, IUserService
    {
        private readonly IUserData userData;

        public UserService(IUserData userData) : base(userData)
        {
            this.userData = userData;
        }

        //自行扩展或重写
        public OperateResult<User> FindByUserNo(long userNo)
        {
            var data = userData.FindByUserNo(userNo);
            return OperateResult<User>.Success(data);
        }

        public OperateResult<int> UpdatePositionName(Position position)
        {
            var data = userData.UpdatePositionName(position);
            return OperateResult<int>.Success(data);
        }

        public OperateResult<int> UpdateDepartment(Department department)
        {
            var data = userData.UpdateDepartment(department);
            return OperateResult<int>.Success(data);
        }

        public OperateResult<int> UpdateSubDepartment(SubDepartment subDepartment)
        {
            var data = userData.UpdateSubDepartment(subDepartment);
            return OperateResult<int>.Success(data);
        }

        public OperateResult<int> UpdateTeam(Team team)
        {
            var data = userData.UpdateTeam(team);
            return OperateResult<int>.Success(data);
        }

        public OperateResult<object> CheckRealName(List<string> realNames)
        {
            var userResult = SearchRequest(new SearchRequest<UserModel>(new UserModel()
            {
                RealNames = realNames
            }, null, -1));

            if (!userResult.Ok())
            {
                return OperateResult<object>.Failed(userResult.Message);
            }

            if (userResult.Data != null && userResult.Data.Count > 0) { }
            else
            {
                return OperateResult<object>.Failed("根据姓名查询不到任何数据");
            }

            var noExistsUser = realNames.Where(c => !userResult.Data.Select(d => d.RealName).Contains(c)).ToList();
            if (noExistsUser != null && noExistsUser.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (var m in noExistsUser)
                {
                    sb.AppendFormatNewLine("员工姓名【{0}】不存在，", m);
                }
                sb.AppendFormatNewLine("请先添加");
                return OperateResult<object>.Failed(sb.ToString());
            }
            return OperateResult<object>.Success(userResult.Data);
        }

        public SearchResult<List<User>> SearchByRealNameDepartmentAndUserNo(List<User> users)
        {
            var data = userData.SearchByRealNameDepartmentAndUserNo(users);
            return base.SearchResult(data);
        }

        public OperateResult<List<User>> CheckRealNameDepartmentAndUserNo(List<User> users, List<User> users2 = null)
        {
            if (users2 == null)
                users2 = userData.SearchByRealNameDepartmentAndUserNo(users);

            var sb = new StringBuilder();
            var success = true;
            foreach (var u in users)
            {
                var userList = users2.Where(u2 => u2.RealName == u.RealName && u2.DepartmentName == u.DepartmentName && (u.UserNo < 1 || u.UserNo == u2.UserNo)).ToList();
                if (userList == null || userList.Count == 0)
                {
                    success = false;
                    /*
                    if (!users2.Exists(u2 => u2.RealName == u.RealName))
                    {
                        sb.AppendFormatNewLine("姓名{0}不存在", u.RealName);
                        continue;
                    }
                    if (!users2.Exists(u2 => u2.DepartmentName == u.DepartmentName))
                    {
                        sb.AppendFormatNewLine("部门{0}不存在", u.DepartmentName);
                        continue;
                    }*/
                    sb.AppendFormatNewLine("用户姓名{0}、部门{1}、工号{2}不存在或不匹配", u.RealName, u.DepartmentName, u.UserNo);
                    continue;
                }
                else if (userList.Count > 1)
                {
                    success = false;
                    sb.AppendFormatNewLine("用户姓名{0}、部门{1}、工号{2}出现多个匹配用户", u.RealName, u.DepartmentName, u.UserNo);
                    continue;
                }
            }
            return success ? OperateResult<List<User>>.Success(users2) : OperateResult<List<User>>.Failed(sb.ToString());
        }
    }
}
