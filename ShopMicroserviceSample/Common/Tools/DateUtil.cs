using System;

namespace Common.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateUtil
    {
        /// <summary>
        /// 从整数转换为日期
        /// </summary>
        /// <param name="date">yyyyMM或yyyyMMdd格式</param>
        /// <returns></returns>
        public static DateTime GetDate(int date)
        {
            var s = date.ToString();
            if (s.Length == 6)
            {
                s += "01";
            }
            s = s.Insert(4, "-").Insert(7, "-");
            return DateTime.Parse(s);
        }

        /// <summary>
        /// 时间是否在某个时间之后
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="pastTime"></param>
        /// <param name="equalAsWell"></param>
        /// <returns></returns>
        public static bool IsAfter(this DateTime dateTime, DateTime pastTime, bool equalAsWell = false)
        {
            if (equalAsWell)
                return dateTime >= pastTime;
            else
                return dateTime > pastTime;
        }

        /// <summary>
        /// 时间是否在某个时间之前
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="futureTime"></param>
        /// <param name="equalAsWell"></param>
        /// <returns></returns>
        public static bool IsBefore(this DateTime dateTime, DateTime futureTime, bool equalAsWell = false)
        {
            if (equalAsWell)
                return dateTime <= futureTime;
            else
                return dateTime < futureTime;
        }

        /// <summary>
        /// 日期转换为短日期yyyy-MM-dd
        /// </summary>
        /// <param name="dateTime">原始日期</param>
        /// <returns>短日期yyyy-MM-dd</returns>
        public static DateTime GetShortDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 日期转换为全日期yyyy-MM-dd HH:mm:ss.fff字符串
        /// </summary>
        /// <param name="dateTime">原始日期</param>
        /// <returns>长日期yyyy-MM-dd HH:mm:ss.fff字符串</returns>
        public static string GetFullDateString(this DateTime dateTime)
        {
            if (dateTime == null) return "";
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 日期转换为长日期yyyy-MM-dd HH:mm:ss字符串
        /// </summary>
        /// <param name="dateTime">原始日期</param>
        /// <returns>长日期yyyy-MM-dd HH:mm:ss字符串</returns>
        public static string GetLongDateString(this DateTime dateTime)
        {
            if (dateTime == null) return "";
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 日期转换为短日期yyyy-MM-dd字符串
        /// </summary>
        /// <param name="dateTime">原始日期</param>
        /// <returns>短日期yyyy-MM-dd字符串</returns>
        public static string GetShortDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 日期转换为短日期yyyyMMdd整数
        /// </summary>
        /// <param name="dateTime">原始日期</param>
        /// <returns>短日期yyyyMMdd整数</returns>
        public static int GetShortDateInteger(this DateTime dateTime)
        {
            return int.Parse(dateTime.ToString("yyyyMMdd"));
        }

        /// <summary>
        /// 日期转换为短日期yyyyMM整数
        /// </summary>
        /// <param name="dateTime">原始日期</param>
        /// <returns>短日期yyyyMM整数</returns>
        public static int GetYearMonthInteger(this DateTime dateTime)
        {
            return int.Parse(dateTime.ToString("yyyyMM"));
        }

        /// <summary>
        /// 两个日期相差的天数
        /// </summary>
        /// <param name="dateEnd">结束日期</param>
        /// <param name="dateStart">开始日期</param>
        /// <param name="nationalDays">是否自然日，如果是，那么 2020-1-2 12:00:00 和 2020-1-1 18:00相差1天，否则0天</param>
        /// <returns>相差天数</returns>
        public static int DayDiffer(this DateTime dateEnd, DateTime dateStart, bool nationalDays = true)
        {
            var date1 = dateEnd;
            var date2 = dateStart;
            if (nationalDays)
            {
                date1 = new DateTime(date1.Year, date1.Month, date1.Day);
                date2 = new DateTime(date2.Year, date2.Month, date2.Day);
            }
            
            return date1.Subtract(date2).Days;
        }

        /// <summary>
        /// 两个日期相差的月份数
        /// </summary>
        /// <param name="dateEnd">结束日期</param>
        /// <param name="dateStart">开始日期</param>
        /// <returns>相差月份</returns>
        public static int MonthDiffer(this DateTime dateEnd, DateTime dateStart)
        {
            return (dateEnd.Year - dateStart.Year) * 12 + dateEnd.Month - dateStart.Month;
        }

        /// <summary>
        /// 一个日期所在周的第一天（周一）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime FirstDateOfWeek(this DateTime dateTime)
        {
            //星期一为第一天
            int weeknow = Convert.ToInt32(dateTime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天
            return dateTime.AddDays(daydiff);
        }

        /// <summary>
        /// 一个日期所在月的第一天（1号）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>

        public static DateTime FirstDateOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// 一个日期所在年的第一天（1号）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime FirstDateOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        /// <summary>
        /// 获取某月的天数
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static int GetMonthDays(int month)
        {
            return GetDate(month).AddMonths(1).AddSeconds(-1).GetShortDateInteger() - month * 100;
        }

        /// <summary>
        /// 获取某月的天数
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static int GetMonthDays(DateTime month)
        {
            return GetMonthDays(month.GetYearMonthInteger());
        }

        /// <summary>
        /// 取得当月的最后一天
        /// </summary> 
        /// <returns></returns> 
        public static int LastDayOfMonth(DateTime date)
        {
            return int.Parse(date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1).ToString("yyyyMMdd"));
        }

    }
}
