using Excel = Microsoft.Office.Interop.Excel;

namespace Common.Helper
{
    /// <summary>
    /// Excel 操作工具
    /// </summary>
    public abstract class AbstractExcelUtil
    {
        /// <summary>
        ///  Excel 程序载体
        /// </summary>       
        Excel.Application excelApp = null;

        /// <summary>
        /// Excel 工作本
        /// </summary>
        Excel.Workbook excelWorkbook = null;

        /// <summary>
        /// Excel 单元表
        /// </summary>
        Excel.Worksheet excelWorksheel = null;


        /// <summary>
        /// 构造方法 
        /// </summary>
        public AbstractExcelUtil()
        {

        }
   
        /// <summary>
        /// 创建 Excel 工作本 
        /// </summary>
        protected void CreateWorkbook()
        {
            excelApp = new Excel.Application();
            excelWorkbook = excelApp.Application.Workbooks.Add();
        }

        /// <summary>
        /// 打开现有的 Excel 工作本
        /// </summary>
        /// <param name="filePath">工作本路径</param>
        protected void OpenWorkbook(string filePath)
        {
            excelApp = new Excel.Application();
            excelWorkbook = excelApp.Application.Workbooks.Open(filePath);
        }

        /// <summary>
        /// 添加一个Worksheet
        /// </summary>
        /// <param name="sheelName">Worksheel 名称</param>
        /// <returns>成功返回True表示添加Worksheel成功，失败返回False表示添加Worksheel失败输入参数sheelName不正确、没有创建 Excel 工作本 </returns>
        protected bool AddWorksheet(string sheelName)
        {
            if (string.IsNullOrWhiteSpace(sheelName) == true)
            {
                return false;
            }
            if (excelWorkbook == null)
            {
                return false;
            }
            excelWorksheel = (Excel.Worksheet)excelWorkbook.Worksheets.Add();
            excelWorksheel.Name = sheelName;
            return true;
        }

        /// <summary>
        /// 设定当前工作表
        /// </summary>
        /// <param name="sheelIndex">工作表索引</param>
        protected void SetWorksheet(int sheelIndex)
        {
            excelWorksheel = (Excel.Worksheet)excelWorkbook.Worksheets[sheelIndex];
        }

        /// <summary>
        /// 设定当前工作表
        /// </summary>
        /// <param name="sheelName">工作表名称</param>
        protected void SetWorksheet(string sheelName)
        {
            foreach (Excel.Worksheet item in excelWorkbook.Worksheets)
            {
                if (item.Name.Equals(sheelName) == true)
                {
                    excelWorksheel = item;
                    break;
                }
            }
        }

        /// <summary>
        /// 給单元格赋值
        /// </summary>
        /// <param name="rowIndex">行</param>
        /// <param name="columnIndex">列</param>
        /// <param name="value">值</param>
        protected void SetCellValue(int rowIndex, int columnIndex, object value)
        {
            excelWorksheel.Cells[rowIndex, columnIndex] = value;
        }

        /// <summary>
        /// 获取单元格的值
        /// </summary>
        /// <param name="rowIndex">行</param>
        /// <param name="columnIndex">列</param>
        /// <returns>单元格的值</returns>
        protected dynamic GetCellValue(int rowIndex, int columnIndex)
        {
            return ((Excel.Range)excelWorksheel.Cells[rowIndex, columnIndex]).Value;
        }

        /// <summary>
        /// 获取单元格的数值类型的值【int、double、DateTime、Currency等类型】
        /// </summary>
        /// <param name="rowIndex">行</param>
        /// <param name="columnIndex">列</param>
        /// <returns>单元格的值</returns>
        protected dynamic GetCellValue2(int rowIndex, int columnIndex)
        {
            return ((Excel.Range)excelWorksheel.Cells[rowIndex, columnIndex]).Value2;
        }

        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="rowIndex">行</param>
        /// <param name="columnIndex">列</param>
        /// <returns>返回当前单元格</returns>
        protected Excel.Range GetCell(int rowIndex, int columnIndex)
        {
            return (Excel.Range)excelWorksheel.Cells[rowIndex, columnIndex];
        }

        /// <summary>
        /// 合拼单元格
        /// </summary>
        /// <param name="rowIndex1">起始单格的行</param>
        /// <param name="columnIndex1">起始单格的列</param>
        /// <param name="rowIndex2">结束单格的行</param>
        /// <param name="columnIndex2">结束单格的列</param>
        /// <returns>返回合拼后的单元格</returns>
        protected Excel.Range MergerCells(int rowIndex1, int columnIndex1, int rowIndex2, int columnIndex2)
        {
            Excel.Range range = excelWorksheel.Range[excelWorksheel.Cells[rowIndex1, columnIndex1], excelWorksheel.Cells[rowIndex2, columnIndex2]];
            range.MergeCells = true;
            return range;
        }

        /// <summary>
        /// 合拼单元格
        /// </summary>
        /// <param name="startCell">起始单格</param>
        /// <param name="endCell">结束单格</param>
        /// <returns>返回合拼后的单元格</returns>
        protected Excel.Range MergerCells(Excel.Range startCell, Excel.Range endCell)
        {
            Excel.Range range = excelWorksheel.Range[startCell, endCell];
            range.MergeCells = true;
            return range;
        }

        /// <summary>
        /// 保存文件并关闭对象
        /// </summary>
        /// <param name="filePath">保存文件路径</param>
        /// <returns>成功返回True表示保存成功，失败返回False表示输入参数filePath不正确</returns>
        protected bool SaveAndCloseWorkbook(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) == true)
            {
                return false;
            }
            excelWorkbook.SaveAs(filePath);
            excelWorkbook.Close();
            excelApp.Quit();
            return true;
        }

        /// <summary>
        /// 释放所有 Excel 进程
        /// </summary>
        /// <returns>成功返回True表示已经释放所有 Excel 进程，失败返回False表示释放所有 Excel 进程发生错误</returns>
        protected bool KillAllExcel()
        {
            try
            {
                if (excelApp == null)
                {
                    return true;
                }
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                {
                    if (process.CloseMainWindow() == false)
                    {
                        process.Kill();
                    }
                }
                excelApp = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
