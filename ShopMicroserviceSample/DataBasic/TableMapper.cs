using Common;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBasic
{
    public class TableMapper<T1>
    {
        private string insertFileds = null;
        private string insertValues = null;

        private string selectFileds = null;
        private string selectMappingFileds = null;

        public TableMapper()
        {
            this.InsertReturnIdentity = true;
            this.UpdateNull = false;
        }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表简称（跨表查询）
        /// </summary>
        public string ShortTableName { get; set; }

        /// <summary>
        /// 主键表达式，如user_id={0}或组合user_id={0} and user_name='{1}'
        /// </summary>
        public string PrimaryKeys { get; set; }

        /// <summary>
        /// 所有列名
        /// </summary>
        public List<string> OriginalFileds { get; set; }

        /// <summary>
        /// 映射实体对象属性，如和实体一样，赋值为null
        /// </summary>
        public List<string> MappingFileds { get; set; }

        /// <summary>
        /// 插入时忽略的列
        /// </summary>
        public List<string> InsertIgnoreFileds { get; set; }

        /// <summary>
        /// 插入时附加条件
        /// </summary>
        public string InsertWhere { get; set; }

        /// <summary>
        /// 插入时是否返回自增主键
        /// </summary>
        public bool InsertReturnIdentity { get; set; }

        /// <summary>
        /// 更新时忽略的列
        /// </summary>
        public List<string> UpdateIgnoreFileds { get; set; }

        /// <summary>
        /// 更新时附加条件，如user_id=@UserId 或 user_id=@UserId and user_name=@UserName
        /// </summary>
        public string UpdateWhere { get; set; }

        /// <summary>
        /// 更新时null的值是否也更新
        /// </summary>
        public bool UpdateNull { get; set; }

        /// <summary>
        /// 分页默认排序，必填
        /// </summary>
        public string DefaultOrderBy { get; set; }

        /// <summary>
        /// 插入列名
        /// </summary>
        public string InsertFileds
        {
            get
            {
                if (!string.IsNullOrEmpty(insertFileds)) return insertFileds;

                StringBuilder sb = new StringBuilder();
                foreach (string f in this.OriginalFileds)
                {
                    if (this.InsertIgnoreFileds==null || !this.InsertIgnoreFileds.Contains(f))
                    {
                       sb.AppendFormat("[{0}], ", f);
                    }
                }
                var _insertNames  = sb.ToString();

                if (!string.IsNullOrEmpty(_insertNames))
                {
                    insertFileds = _insertNames.TrimEnd(new char[] { ' ', ',' });
                }

                return insertFileds;
            }
        }

        /// <summary>
        /// 插入值
        /// </summary>
        public string InsertValues
        {
            get
            {
                if (!string.IsNullOrEmpty(insertValues)) return insertValues;

                StringBuilder sb = new StringBuilder();
                for (var i = 0; i < this.OriginalFileds.Count; i++)
                {
                    if (this.InsertIgnoreFileds == null || !this.InsertIgnoreFileds.Contains(this.OriginalFileds[i]))
                    {
                        sb.AppendFormat("@{0}, ", this.MappingName(i));
                    }
                }

                var _insertValues = sb.ToString();
                if (!string.IsNullOrEmpty(_insertValues))
                {
                    insertValues = _insertValues.TrimEnd(new char[] { ' ', ',' });
                }
                return insertValues;
            }
        }

        /// <summary>
        /// 筛选列
        /// </summary>
        public string SelectFileds
        {
            get
            {
                if (!string.IsNullOrEmpty(selectFileds)) return selectFileds;

                StringBuilder sb = new StringBuilder();

                for (var i = 0; i < this.OriginalFileds.Count; i++)
                {
                    sb.AppendFormat("[{0}]{1}, ", this.OriginalFileds[i], this.MappingName(i, " as "));
                }

                var _selectFileds = sb.ToString();
                if (!string.IsNullOrEmpty(_selectFileds))
                {
                    selectFileds = _selectFileds.TrimEnd(new char[] { ' ', ',' });
                }
                return selectFileds;
            }
        }

        /// <summary>
        /// 筛选映射过的列
        /// </summary>
        public string SelectMappingFileds
        {
            get
            {
                if (!string.IsNullOrEmpty(selectMappingFileds)) return selectMappingFileds;

                if (this.MappingFileds == null || this.MappingFileds.Count == 0)
                {
                    selectMappingFileds = this.SelectFileds;
                }
                else
                {
                    selectMappingFileds = string.Join(",", this.MappingFileds.ToArray());
                }

                return selectMappingFileds;
            }
        }

        /// <summary>
        /// 更新字符串拼接
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string UpdateString(T1 entity)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < this.OriginalFileds.Count; i++)
            {
                var originalFiled = this.OriginalFileds[i];
                if (this.UpdateIgnoreFileds.Contains(originalFiled)) continue;

                var name = this.MappingName(i);
                if (this.UpdateNull)
                {
                    sb.AppendFormat("[{0}]=@{1}, ", originalFiled, name);
                }
                else
                {
                    var value = ObjectUtil.GetPropertyValue(entity,name); 
                    if (value == null) continue;

                    if (Constants.NullValues.Contains(value.ToString()))
                    {
                        sb.AppendFormat("[{0}]=null, ", originalFiled);
                    }
                    else
                    {
                        sb.AppendFormat("[{0}]=@{1}, ", originalFiled, name);
                    }
                }
            }
            var _updateString = sb.ToString();

            if (_updateString != "")
            {
                _updateString = _updateString.TrimEnd(new char[] {' ',   ','});
            }
            return _updateString;
        }

        /// <summary>
        /// 获得映射字段名
        /// </summary>
        /// <param name="index"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public string MappingName(int index, string prefix = "")
        {
            if (this.MappingFileds == null || this.MappingFileds.Count == 0 || string.IsNullOrEmpty(this.MappingFileds[index]))
            {
                return prefix == " as " ? "" : this.OriginalFileds[index];
            }
            else
            {
                return string.Format("{0}{1}", prefix, this.MappingFileds[index]);
            }
        }

        public string SelectFiledsWithTableName(string tableName=null)
        {
            if (string.IsNullOrEmpty(tableName)) tableName = this.ShortTableName;

            var _selectFileds =this.SelectFileds;
            if (string.IsNullOrEmpty(tableName)) return _selectFileds;

            return string.Format("[{0}].{1}", tableName, _selectFileds.Replace(",", $", [{tableName}]."));
        }
    }
}
