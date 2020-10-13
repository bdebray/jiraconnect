using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Data.Csv;
using JiraWriter.Model;
using JiraWriter.Extension;

namespace JiraWriterTest.Data.Csv
{
    [TestClass]
    public class JiraIssueCsvMapperTest
    {
        protected readonly JiraIssue _validJiraIssue = new JiraIssue("TEST_KEY", "TEST_DESCRIPTION")
        {
            Type = "TEST_TYPE",
            Team = "TEST_TEAM",
            Status = "TEST_STATUS",
            Labels = new string[] { "TEST_LABELS_1", "TEST_LABELS_2" },
            InProgressDate = DateTime.Today,
            DoneDate = DateTime.Today.AddDays(1),
            CycleTime = 2,
            TimeInStates = new List<TimeInState>
                {
                    new TimeInState("TEST_STATE_1", DateTime.Today),
                    new TimeInState("TEST_STATE_2", DateTime.Today.AddDays(1))
                }
        };

        [TestMethod]
        public void ShouldMapUnformattedJiraIssueFields()
        {
            var target = JiraIssueCsvMapper.GetCsvFields(_validJiraIssue);

            Assert.AreEqual(_validJiraIssue.Key, target.GetValueOrDefault("Key"));
            Assert.AreEqual(_validJiraIssue.Description, target.GetValueOrDefault("Description"));
            Assert.AreEqual(_validJiraIssue.Type, target.GetValueOrDefault("Type"));
            Assert.AreEqual(_validJiraIssue.Team, target.GetValueOrDefault("Team"));
            Assert.AreEqual(_validJiraIssue.Status, target.GetValueOrDefault("Status"));
            Assert.AreEqual(_validJiraIssue.CycleTime.ToString(), target.GetValueOrDefault("CycleTime"));
        }

        [TestMethod]
        public void ShouldMapFormattedTimeDateFields()
        {
            var target = JiraIssueCsvMapper.GetCsvFields(_validJiraIssue);

            Assert.AreEqual(_validJiraIssue.InProgressDate.ToDisplayDate(), target.GetValueOrDefault("InProgressDate"));
            Assert.AreEqual(_validJiraIssue.DoneDate.ToDisplayDate(), target.GetValueOrDefault("DoneDate"));
        }

        [TestMethod]
        public void ShouldMapUnsetDateFieldsToBlank()
        {
            _validJiraIssue.InProgressDate = DateTime.MinValue;
            _validJiraIssue.DoneDate = DateTime.MinValue;

            var target = JiraIssueCsvMapper.GetCsvFields(_validJiraIssue);

            Assert.AreEqual(string.Empty, target.GetValueOrDefault("InProgressDate"));
            Assert.AreEqual(string.Empty, target.GetValueOrDefault("DoneDate"));
        }

        [TestMethod]
        public void ShouldMapEmptyCycleTimeToBlank()
        {
            _validJiraIssue.CycleTime = null;

            var target = JiraIssueCsvMapper.GetCsvFields(_validJiraIssue);

            Assert.AreEqual(string.Empty, target.GetValueOrDefault("CycleTime"));
        }

        [TestMethod]
        public void ShouldMapTimeInStateDates()
        {
            var target = JiraIssueCsvMapper.GetCsvFields(_validJiraIssue);

            _validJiraIssue.TimeInStates.ForEach(state =>
            {
                Assert.AreEqual(state.Date.ToDisplayDate(), target.GetValueOrDefault(state.StateName));
            });
        }
    }
}
