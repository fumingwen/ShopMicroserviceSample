using Common;
using Common.Database;
using SalaryService.Entity;
using ServiceBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Service
{
    public interface IUserService : IServiceBasic<User, long, UserModel>
    {
        //自行扩展或重写
        /// <summary>
        /// 根据用户工号查询用户信息
        /// </summary>
        /// <param name="userNo">用户工号</param>
        /// <returns></returns>
        OperateResult<User> FindByUserNo(long userNo);

        /// <summary>
        /// 根据职位ID号更新职位名称
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        OperateResult<int> UpdatePositionName(Position position);

        /// <summary>
        /// 更新部门信息时同步更新用户部门
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        OperateResult<int> UpdateDepartment(Department department);

        /// <summary>
        /// 更新系列信息时同步更新用户系列
        /// </summary>
        /// <param name="subDepartment"></param>
        /// <returns></returns>
        OperateResult<int> UpdateSubDepartment(SubDepartment subDepartment);

        /// <summary>
        /// 更新团队信息时同步更新用户团队
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        OperateResult<int> UpdateTeam(Team team);

        /// <summary>
        /// 根据用户姓名检查用户是否存在
        /// </summary>
        /// <param name="realNames"></param>
        /// <returns>检查通过返回用户列表</returns>
        OperateResult<object> CheckRealName(List<string> realNames);

        /// <summary>
        /// 通过导入的用户信息（姓名，部门、工号）查询用户
        /// </summary>
        /// <param name="users">用户信息（姓名，部门、工号）</param>
        /// <returns></returns>
        SearchResult<List<User>> SearchByRealNameDepartmentAndUserNo(List<User> users);

        /// <summary>
        /// 验证导入的用户信息（姓名，部门、工号）查询用户
        /// </summary>
        /// <param name="users">用户信息（姓名，部门、工号）</param>
        /// <param name="users2">根据导入条件从数据库查询出来的用户信息，如为null则重新查询</param>
        /// <returns></returns>
        OperateResult<List<User>> CheckRealNameDepartmentAndUserNo(List<User> users, List<User> users2 = null);
    }
}
