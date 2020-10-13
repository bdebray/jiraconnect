using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Extension;

namespace JiraWriterTest.Extension
{
    [TestClass]
    public class DateTimeExtensionsTest
    {
        protected DateTime _startDate = DateTime.Today;

        [TestMethod]
        public void ShouldGetNumberOfDaysFromStartToEndDate()
        {
            var endDate = _startDate.AddDays(3);

            var target = _startDate.NumberOfDays(endDate);

            Assert.AreEqual(4, target);
        }

        [TestMethod]
        public void ShouldIgnoreSpecifiedDaysOfWeekInNumberOfDays()
        {
            var endDate = _startDate.AddDays(2);
            var dateIncrement = _startDate;
            var daysOfWeekToExclude = new List<DayOfWeek>();

            while (dateIncrement < endDate)
            {
                daysOfWeekToExclude.Add(dateIncrement.DayOfWeek);

                dateIncrement = dateIncrement.AddDays(1);
            }

            var target = _startDate.NumberOfDays(endDate, daysOfWeekToExclude);

            //ignored all but the end date
            Assert.AreEqual(1, target);
        }

        [TestMethod]
        public void ShouldIgnoreHoursInNumberOfDaysCalculation()
        {
            var startDateWithTime = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, 23, 59, 59);
            var endDate = startDateWithTime.AddMinutes(2);

            var target = startDateWithTime.NumberOfDays(endDate);

            //still includes the start date and end date, even though it is a 2 minute difference
            Assert.AreEqual(2, target);
        }

        [TestMethod]
        public void ShouldCountSameStartAndEndDateAsOneDay()
        {
            var endDate = _startDate;

            var target = _startDate.NumberOfDays(endDate);

            Assert.AreEqual(1, target);
        }

        [TestMethod]
        public void ShouldShowCorrectDisplayDate()
        {
            Assert.AreEqual(_startDate.ToShortDateString(), _startDate.ToDisplayDate());
        }

        [TestMethod]
        public void ShouldShowEmptyStringForDisplayDateWhenDefaultMinDate()
        {
            Assert.AreEqual("", DateTime.MinValue.ToDisplayDate());
        }
    }
}
