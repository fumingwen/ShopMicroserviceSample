using System.Collections.Generic;

namespace Common.ExcelTool
{
    /// <summary>
    /// Excel导入的映射
    /// </summary>
    public class ExcelMapper
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 映射字典
        /// </summary>
        public Dictionary<string, object> Mapper { get; set; }

        /// <summary>
        /// 不允许空的列
        /// </summary>
        public List<int> NotNullColumns { get; set; }

        /// <summary>
        /// 标题占的行数
        /// </summary>
        public int TitleRows { get; set; }

        /// <summary>
        /// 最后一个属性合并开始
        /// </summary>
        public int LastFrom { get; set; }

        /// <summary>
        /// 最后一个属性合并结束
        /// </summary>
        public int LastEnd { get; set; }
    }
}
