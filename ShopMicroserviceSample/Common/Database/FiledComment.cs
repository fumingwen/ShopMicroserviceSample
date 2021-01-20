using System;

namespace Common.Database
{
    /// <summary>
    /// 针对数据库的属性注解
    /// </summary>
    public class FiledComment : Attribute
    {
        /// <summary>
        /// 跨表查询指定字段取自哪个表
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 缩写名称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 字段名称，如果和数据库不一致
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否自动组装where查询语句，默认true
        /// </summary>
        public bool AutoWhere { get; set; }

        /// <summary>
        /// 字符串用=查询，默认是like
        /// </summary>
        public bool StringEqual { get; set; }

        /// <summary>
        /// 忽略该字段（可能不是数据库的字段)
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// 插入忽略该字段
        /// </summary>
        public bool InsertIgnore { get; set; }

        /// <summary>
        /// 更新忽略该字段
        /// </summary>
        public bool UpdateIgnore { get; set; }

        /// <summary>
        /// 属性注解
        /// </summary>
        public FiledComment()
        {
            this.AutoWhere = true;
        }

        /// <summary>
        /// 属性注解
        /// </summary>
        /// <param name="tableName">表名称</param>
        public FiledComment(string tableName)
        {
            this.AutoWhere = true;
            this.TableName = tableName;
        }

        /// <summary>
        /// 属性注解
        /// </summary>
        /// <param name="autoWhere">自动组合条件</param>
        /// <param name="stringEqual">字符串精确相等查询</param>
        /// <param name="tableName">表名称</param>
        public FiledComment(bool autoWhere, bool stringEqual = false, string tableName = null)
        {
            this.AutoWhere = autoWhere;
            this.StringEqual = stringEqual;
            this.TableName = tableName;
        }
    }
}
