using System.Collections.Generic;

namespace Common.ExcelTool
{
    /// <summary>
    /// Excel操作接口
    /// </summary>
    public interface IExcel
    {
        /// <summary>
        /// 从Excel生成实体对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="path">Excel绝对路径</param>
        /// <param name="excelMapper">Excel映射配置</param>
        /// <returns>实体对象列表</returns>
        OperateResult<List<T>> FromExcel<T>(string path, ExcelMapper excelMapper);
    }
}
