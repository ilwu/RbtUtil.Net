using System;

namespace rbt.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 將日期轉換民國年字串
        /// </summary>
        /// <param name="date">西元年日期DateTime物件</param>
        public static string ToTwDate(this DateTime date)
        {
            int year = date.Year - 1911;
            return string.Format("{0}/{1:00}/{2:00}", year, date.Month, date.Day);
        }

        /// <summary>
        /// 將日期轉換民國年字串
        /// </summary>
        /// <param name="date">西元年日期DateTime物件</param>
        public static string ToROCDateTimeStr(this DateTime date)
        {
            int year = date.Year - 1911;
            return string.Format("{0:000}{1:00}{2:00}{3:00}{4:00}{5:00}", year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToROCDateStr(this DateTime date)
        {
            int year = date.Year - 1911;
            return string.Format("{0:000}{1:00}{2:00}", year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateStr(this DateTime date)
        {
            return string.Format("{0:0000}{1:00}{2:00}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
    }
}