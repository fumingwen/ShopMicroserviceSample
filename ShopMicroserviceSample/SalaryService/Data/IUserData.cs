using DataBasic;
using SalaryService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryService.Data
{
    public interface IUserData : IDataBasic<User, long, UserModel>
    {
        //自行扩展或重写
        /// <summary>
        /// 根据用户工号查询用户信息
        /// </summary>
        /// <param name="userNo">用户工号</param>
        /// <returns></returns>
        User FindByUserNo(long userNo);

        /// <summary>
        /// 根据职位ID号更新职位名称
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        int UpdatePositionName(Position position);

        /// <summary>
        /// 更新部门信息时同步更新用户部门
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        int UpdateDepartment(Department department);

        /// <summary>
        /// 更新系列信息时同步更新用户系列
        /// </summary>
        /// <param name="subDepartment"></param>
        /// <returns></returns>
        int UpdateSubDepartment(SubDepartment subDepartment);

        /// <summary>
        /// 更新团队信息时同步更新用户团队
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        int UpdateTeam(Team team);

        /// <summary>
        /// 通过导入的用户信息（姓名，部门、工号）查询用户
        /// </summary>
        /// <param name="users">用户信息（姓名，部门、工号）</param>
        /// <returns></returns>
        List<User> SearchByRealNameDepartmentAndUserNo(List<User> users);
    }
}
