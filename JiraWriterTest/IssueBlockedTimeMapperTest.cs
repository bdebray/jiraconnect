using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Model;
using JiraWriter;

namespace JiraWriterTest
{
    [TestClass]
    public class IssueBlockedTimeMapperTest
    {
        [TestMethod]
        public void ShouldHaveBlockedTimeFromSingleBlock()
        {
            var expectedDurationDays = 2;

            var blocks = new List<Block>()
            {
                new Block(new DateTime(2020,10,7), new DateTime(2020,10,8))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks,
                Type = "TEST_TYPE"
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldHaveBlockedTimeFromMultipleBlocks()
        {
            var expectedDurationDays = 5;

            var blocks = new List<Block>()
            {
                //2 days
                new Block(new DateTime(2020,10,7), new DateTime(2020,10,8)),
                //3 days
                new Block(new DateTime(2020,10,10), new DateTime(2020,10,12))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldNotHaveBlockedTimeWithNoBlocks()
        {
            var expectedDurationDays = 0;

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION");

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldNotHaveBlockedTimeWithOnlyCurrentActiveBlock()
        {
            var expectedDurationDays = 0;

            var blocks = new List<Block>()
            {
                //current block (.e.g. not unblocked
                new Block(new DateTime(2020,10,7))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldCalculatedBlockedTimeWhenExcludingDays()
        {
            var expectedDurationDays = 3;

            var blocks = new List<Block>()
            {
                //still 3 days, after excluding Saturday and Sunday
                new Block(new DateTime(2020,10,7), new DateTime(2020,10,11))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue, 0, new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday });

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldNotIncludeBlockedTimeForOnlyExcludedDays()
        {
            var expectedDurationDays = 0;

            var blocks = new List<Block>()
            {
                //only blocked on days of the week that are excluded
                new Block(new DateTime(2020,10,9), new DateTime(2020,10,10))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue, 0, new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday });

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldHaveBlockedTimeofOneDayWhenFlaggedForLessThanADayWithNoThreshold()
        {
            var expectedDurationDays = 1;

            var blocks = new List<Block>()
            {
                new Block(new DateTime(2020,10,7), new DateTime(2020,10,7))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks,
                Type = "TEST_TYPE"
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldNotHaveBlockedTimeWhenFlaggedForLessThanADayWithThreshold()
        {
            var thresholdMinutes = 15;
            var expectedDurationDays = 0;

            var blocks = new List<Block>()
            {
                new Block(new DateTime(2020,10,7, 12, 0, 0), new DateTime(2020,10,7, 12, 1, 0))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks,
                Type = "TEST_TYPE"
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue, thresholdMinutes);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldIncludeBlockedTimeWhenFlaggedForMoreThanThreshold()
        {
            var thresholdMinutes = 15;
            var expectedDurationDays = 1;

            var blocks = new List<Block>()
            {
                new Block(new DateTime(2020,10,7, 12, 0, 0), new DateTime(2020,10,7, 12, 16, 0))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks,
                Type = "TEST_TYPE"
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue, thresholdMinutes);

            Assert.AreEqual(expectedDurationDays, target);
        }

        [TestMethod]
        public void ShouldIncludeBlockedTimeWhenFlaggedForSameAsThreshold()
        {
            var thresholdMinutes = 15;
            var expectedDurationDays = 1;
            var blocks = new List<Block>()
            {
                new Block(new DateTime(2020,10,7, 12, 0, 0), new DateTime(2020,10,7, 12, 15, 0))
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                Blocks = blocks,
                Type = "TEST_TYPE"
            };

            var target = IssueBlockedTimeMapper.GetBlockedTime(testIssue, thresholdMinutes);

            Assert.AreEqual(expectedDurationDays, target);
        }
    }
}
