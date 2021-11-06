/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    public static class DateHelper
    {
        public static DateTime Today = DateTime.Now.Date;

        #region property MinDate
        private static DateTime _MinDate = new DateTime(1900, 1, 1);
        public static DateTime MinDate
        {
            get
            {
                return _MinDate;
            }
        }
        #endregion //property MinDate

        public static bool IsMinDate(this DateTime date)
        {
            return date.Equals(MinDate);
        }

        public static DateTime AddTime(this DateTime date, string time)
        {
            if (string.IsNullOrEmpty(time) || time.Length == 1)
            {
                return date;
            }

            var unit = time[time.Length - 1];
            var add = Convert.ToDouble(time.Substring(0, time.Length - 1));
            switch (unit)
            {
                case 'Y':
                case 'y':
                    return date.AddYears((int)add);
                case 'D':
                case 'd':
                    return date.AddDays(add);
                case 'H':
                case 'h':
                    return date.AddHours(add);
                case 'M':
                    return date.AddMonths((int)add);
                case 'm':
                    return date.AddMinutes(add);
                case 'S':
                case 's':
                    return date.AddSeconds(add);
                default:
                    break;
            }

            return date;
        }
    }
}
