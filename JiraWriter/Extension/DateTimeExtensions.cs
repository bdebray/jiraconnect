using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraWriter.Extension
{
    public static class DateTimeExtensions
    {
        public static int NumberOfDays(this DateTime startDate, DateTime endDate, IEnumerable<DayOfWeek> daysOfWeekToExclude = null)
        {
            if (startDate > endDate) return 0;

            if (daysOfWeekToExclude == null) daysOfWeekToExclude = new List<DayOfWeek>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            var tempDate = startDate;
            var numberOfDays = 0;

            while(tempDate <= endDate)
            {
                if (!daysOfWeekToExclude.Contains(tempDate.DayOfWeek)) numberOfDays++;

                tempDate = tempDate.Add(new TimeSpan(1, 0, 0, 0));
            }

            return numberOfDays;
        }

        public static string ToDisplayDate(this DateTime date)
        {
            return (date == null || date.Equals(DateTime.MinValue)) ? string.Empty : date.ToShortDateString();
        }
    }
}
