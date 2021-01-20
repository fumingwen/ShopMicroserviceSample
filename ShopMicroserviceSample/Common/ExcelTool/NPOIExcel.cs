using Common.Helper;
using Common.Tools;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Common.ExcelTool
{
    /// <summary>
    /// Excel 操作工具
    /// </summary>
    public class NPOIExcel : IExcel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="excelMapper"></param>
        /// <returns></returns>
        public OperateResult<List<T>> FromExcel<T>(string path, ExcelMapper excelMapper)
        {
            if (excelMapper == null)
            {
                return OperateResult<List<T>>.Failed("映射配置不存在");
            }

            if (!File.Exists(path))
            {
                return OperateResult<List<T>>.Failed("文件不存在");
            }

            try
            {
                using (FileStream fs = File.OpenRead(path))
                {

                    IWorkbook workbook;
                    if (path.ToLower().EndsWith(".xlsx"))
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    else if (path.ToLower().EndsWith(".xls"))
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    else
                    {
                        return OperateResult<List<T>>.Failed("文件格式不符合");
                    }
                    if (workbook == null || workbook.NumberOfSheets < 1)
                    {
                        return OperateResult<List<T>>.Failed("打开文件有错或无内容");
                    }

                    ISheet worksheel = workbook.GetSheetAt(0);

                    var titleRows = excelMapper.TitleRows;
                    var lastFrom = excelMapper.LastFrom;
                    var lastEnd = excelMapper.LastEnd;

                    var rowCount = worksheel.LastRowNum + 1;
                    if (rowCount < titleRows + 1)
                    {
                        return OperateResult<List<T>>.Failed("行数小于2，无内容");
                    }

                    var mapper = excelMapper.Mapper;
                    var notNullColumns = excelMapper.NotNullColumns;

                    Type type = typeof(T);
                    PropertyInfo[] properties = type.GetProperties();
                    var propertyMapper = new Dictionary<string, PropertyInfo>();

                    foreach (var key in mapper.Keys)
                    {
                        var p = properties.FirstOrDefault(pr => pr.Name == key);
                        if (p != null)
                        {
                            propertyMapper.Add(key, p);
                        }
                    }

                    var list = new List<T>();
                    for (var i = titleRows; i < rowCount; i++)
                    {
                        T item = Activator.CreateInstance<T>();

                        foreach (var key in mapper.Keys)
                        {
                            try
                            {
                                var s = propertyMapper.TryGetValue(key, out PropertyInfo p);

                                if (s && p != null)
                                {
                                    var index = int.Parse(mapper[key].ToString());
                                    if (index >= 0)
                                    {
                                        ICell cell = worksheel.GetRow(i).GetCell(index);
                                        var value = FormatCellValue(GetCellValue(cell), p);
                                        if (notNullColumns != null && notNullColumns.Contains(index) && (value == null || value.ToString().Trim() == ""))
                                        {
                                            i = rowCount;
                                            break;
                                        }
                                        else
                                        {
                                            p.SetValue(item, value);
                                        }
                                    }
                                    else
                                    {
                                        var lasts = new List<object>();
                                        for (var l = lastFrom; l <= lastEnd; l++)
                                        {
                                            ICell cell = worksheel.GetRow(i).GetCell(l);
                                            var value = FormatCellValue(GetCellValue(cell), p);
                                            lasts.Add(value);
                                        }
                                        p.SetValue(item, lasts);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error(ex, string.Format("行数：{0}，Key：{1}",i, key));
                            }
                        }
                        if (i < rowCount) list.Add(item);
                    }
                    fs.Close();
                    workbook.Close();
                    fs.Dispose();
                    return OperateResult<List<T>>.Success(list);
                }
            }
            catch (Exception ex)
            {
                return OperateResult<List<T>>.Failed(ex.Message);
            }
        }

        private static object FormatCellValue(object value, PropertyInfo property)
        {
            if (value == null) return null;

            string typeName = property.RealTypeName().ToLower();

            var _value = value.ToString().TrimStart().TrimEnd();

            if (_value == "")
            {
                switch (typeName)
                {
                    case "datetime":
                        return DateTime.Now;
                    case "string":
                        return _value;
                    default:
                        return null;
                }
            }
            else
            {
                switch (typeName)
                {
                    case "short":
                        return short.Parse(_value);
                    case "int":
                    case "int32":
                        return int.Parse(_value);
                    case "int64":
                        return long.Parse(_value);
                    case "decimal":
                        return decimal.Parse(_value);
                    case "double":
                        return double.Parse(_value);
                    case "datetime":
                        return DateTime.Parse(_value);
                    default:
                        return value;
                }
            }
        }

        private static object GetCellValue(ICell cell)
        {
            if (cell == null) return null;

            switch (cell.CellType)
            {
                case CellType.Blank:
                    return null;
                case CellType.Numeric:
                    if (NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell))
                        return DateTime.FromOADate(cell.NumericCellValue);
                    else
                        return cell.NumericCellValue;
                default:
                    if(cell.CellType!=CellType.String) cell.SetCellType(CellType.String);
                    return cell.StringCellValue;
            }
        }
    }
}
