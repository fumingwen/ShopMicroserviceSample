using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 系统常量
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 空日期“2099-12-12 12:12:12”，若日期为该值则赋值null
        /// </summary>
        public static DateTime NullDate = new DateTime(2099, 12, 12, 12, 12, 12);

        /// <summary>
        /// 空字符串“NULL”，若字符串为该值则赋值null
        /// </summary>
        public static string NullString = "2099-12-12 12:12:12";

        /// <summary>
        /// 所有需要转换为null的值，若字符串为“NULL”则赋值null，若时间为“2099-12-12 12:12:12”也赋值null
        /// </summary>

        public static List<string> NullValues = new List<string>() { "NULL", "2099-12-12 12:12:12", "2099/12/12 12:12:12" };

        /// <summary>
        /// 系统No
        /// </summary>
        public const long SystemId = 989989989989989989;

        /// <summary>
        /// 肖总No
        /// </summary>
        public const long Xiaoyu = 98998998998998;

        /// <summary>
        /// 统计首页销售数据概况的Redis Key
        /// </summary>
        public const string SaleSummaryKey = "salesummary";

        /// <summary>
        /// 无效Token的Redis Key
        /// </summary>
        public const string TokenInvalidKey = "token";

        /// <summary>
        /// 下载文件的Redis Key
        /// </summary>
        public const string DownloadFileKey = "download";

        /// <summary>
        /// 允许上传的文件后缀
        /// </summary>
        public const string UploadExtensions = ".jpg,.git,.png,.bmp,.doc,.docx,.xml,.xls,.xlsx,.html,.htm";

        /// <summary>
        /// 基本类型
        /// </summary>
        public static readonly List<string> BasicTypes = new List<string>() { "system.int", "system.long", "system.string", "system.datetime", "system.decimal", "system.int32", "system.int64", "system.bool" };

    }
}
