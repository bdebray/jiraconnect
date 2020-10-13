using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Model;
using JiraWriter.Config;
using JiraWriter;

namespace JiraWriterTest
{
    [TestClass]
    public class IssueTimeInStateMapperTest
    {
        private readonly List<WorkflowMap> _mockWorkflowMap = new List<WorkflowMap>()
            {
                new WorkflowMap(new string[]{"TO_DO"}, "STUFF TO DO", StateType.ToDo, "TEST_TYPE", 1),
                new WorkflowMap(new string[]{"DOING"}, "DOING STUFF", StateType.InProgress, "TEST_TYPE", 2),
                new WorkflowMap(new string[]{"VALIDATING"}, "TESTING STUFF", StateType.InProgress, "TEST_TYPE", 3),
                new WorkflowMap(new string[]{"DONE"}, "FINISHED", StateType.Done, "TEST_TYPE", 4)
            };

        [TestMethod]
        public void ShouldHaveOnlyOneResultForEachState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO"),
                new JiraState(new DateTime(2020,10,7), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 8), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 9), "VALIDATING", "DOING"),
                new JiraState(new DateTime(2020,10,10), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 11), "VALIDATING", "DONE"),
                new JiraState(new DateTime(2020, 10, 11), "DONE", "TO_DO"),
                new JiraState(new DateTime(2020, 10, 11), "TO_DO", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap);

            Assert.AreEqual(_mockWorkflowMap.Count, target.Count);

            _mockWorkflowMap.ForEach(map =>
            {
                Assert.AreEqual(1, target.Where(state => state.StateName.Equals(map.MappedState)).Count());
            });
        }

        [TestMethod]
        public void ShouldFindLastToDoState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO"),
                new JiraState(new DateTime(2020, 10, 8), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 9), "DOING", "TO_DO")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap);
            Assert.IsTrue(target.Any(state => state.StateName.Equals("STUFF TO DO") && state.Date.Equals(new DateTime(2020, 10, 9))));
            Assert.IsFalse(target.Any(state => state.StateName.Equals("STUFF TO DO") && state.Date.Equals(new DateTime(2020, 10, 7))));
        }

        [TestMethod]
        public void ShouldFindLastDoneState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "DOING", "DONE"),
                new JiraState(new DateTime(2020, 10, 8), "DONE", "TO_DO"),
                new JiraState(new DateTime(2020, 10, 9), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 10), "DOING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap);
            Assert.IsTrue(target.Any(state => state.StateName.Equals("FINISHED") && state.Date.Equals(new DateTime(2020, 10, 10))));
            Assert.IsFalse(target.Any(state => state.StateName.Equals("FINISHED") && state.Date.Equals(new DateTime(2020, 10, 7))));
        }

        [TestMethod]
        public void ShouldFindEarliestForEachInProgressState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 8), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 9), "VALIDATING", "DOING"),
                new JiraState(new DateTime(2020,10,10), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 11), "VALIDATING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap);
            Assert.IsTrue(target.Any(state => state.StateName.Equals("DOING STUFF") && state.Date.Equals(new DateTime(2020, 10, 7))));
            Assert.IsFalse(target.Any(state => state.StateName.Equals("DOING STUFF") && state.Date.Equals(new DateTime(2020, 10, 9))));
            Assert.IsTrue(target.Any(state => state.StateName.Equals("TESTING STUFF") && state.Date.Equals(new DateTime(2020, 10, 8))));
            Assert.IsFalse(target.Any(state => state.StateName.Equals("TESTING STUFF") && state.Date.Equals(new DateTime(2020, 10, 10))));
        }

        [TestMethod]
        public void ShouldFindEarliestInProgressStateAfterLastToDoState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 8), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 9), "VALIDATING", "TO_DO"),
                new JiraState(new DateTime(2020,10,10), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020,10,11), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 12), "VALIDATING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap);
            Assert.IsTrue(target.Any(state => state.StateName.Equals("DOING STUFF") && state.Date.Equals(new DateTime(2020, 10, 10))));
            Assert.IsFalse(target.Any(state => state.StateName.Equals("DOING STUFF") && state.Date.Equals(new DateTime(2020, 10, 7))));
            Assert.IsTrue(target.Any(state => state.StateName.Equals("TESTING STUFF") && state.Date.Equals(new DateTime(2020, 10, 11))));
            Assert.IsFalse(target.Any(state => state.StateName.Equals("TESTING STUFF") && state.Date.Equals(new DateTime(2020, 10, 8))));
        }

        [TestMethod]
        public void ShouldSetDaysInState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO"),
                new JiraState(new DateTime(2020,10,8), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 10), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 13), "VALIDATING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap, null);

            Assert.AreEqual(2, target.Where(state => state.StateName.Equals("STUFF TO DO")).Select(state => state.DaysInState).First());
            Assert.AreEqual(3, target.Where(state => state.StateName.Equals("DOING STUFF")).Select(state => state.DaysInState).First());
            Assert.AreEqual(4, target.Where(state => state.StateName.Equals("TESTING STUFF")).Select(state => state.DaysInState).First());
        }

        [TestMethod]
        public void ShouldSetDaysInStateWithoutWeekends()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO"),
                new JiraState(new DateTime(2020,10,8), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 10), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 13), "VALIDATING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap, new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday });

            Assert.AreEqual(2, target.Where(state => state.StateName.Equals("STUFF TO DO")).Select(state => state.DaysInState).First());
            Assert.AreEqual(2, target.Where(state => state.StateName.Equals("DOING STUFF")).Select(state => state.DaysInState).First());
            Assert.AreEqual(2, target.Where(state => state.StateName.Equals("TESTING STUFF")).Select(state => state.DaysInState).First());
        }

        [TestMethod]
        public void ShouldNotSetTimeInStateForDoneStates()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020, 10, 7), "DOING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap);

            Assert.AreEqual(0, target.Where(state => state.StateName.Equals("FINISHED")).Select(state => state.DaysInState).First());
        }

        [TestMethod]
        public void ShouldSetDaysInStateWhenSkippingStates()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,1), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 10), "DOING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE"
            };

            var target = IssueTimeInStateMapper.GetTimeInStates(testIssue, _mockWorkflowMap, null);

            Assert.AreEqual(0, target.Where(state => state.StateName.Equals("STUFF TO DO")).Select(state => state.DaysInState).First());
            Assert.AreEqual(10, target.Where(state => state.StateName.Equals("DOING STUFF")).Select(state => state.DaysInState).First());
            Assert.AreEqual(0, target.Where(state => state.StateName.Equals("TESTING STUFF")).Select(state => state.DaysInState).First());
        }

        [TestMethod]
        public void ShouldGetEarliestInProgressStatusForInProgressDate()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 8), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 9), "VALIDATING", "DOING"),
                new JiraState(new DateTime(2020,10,10), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 11), "VALIDATING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "DONE"
            };

            var target = IssueTimeInStateMapper.GetInProgressDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(new DateTime(2020, 10, 7), target);
        }

        [TestMethod]
        public void ShouldGetEarliestInProgressStatusForInProgressDateAfterLastToDoState()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 8), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 9), "VALIDATING", "TO_DO"),
                new JiraState(new DateTime(2020,10,10), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020,10,11), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 12), "VALIDATING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "DONE"
            };

            var target = IssueTimeInStateMapper.GetInProgressDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(new DateTime(2020, 10, 10), target);
        }

        [TestMethod]
        public void ShouldGetDoneDateForInProgressDateWhenOnlyMovedToDone()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO"),
                new JiraState(new DateTime(2020, 10, 8), "TO_DO", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "DONE"
            };

            var target = IssueTimeInStateMapper.GetInProgressDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(new DateTime(2020, 10, 8), target);
        }

        [TestMethod]
        public void ShouldGetDoneDateForInProgressDateWithNoInProgressStateMaps()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 8), "DOING", "VALIDATING"),
                new JiraState(new DateTime(2020, 10, 9), "VALIDATING", "DONE")
            };

            var partialWorkflowMap = new List<WorkflowMap>()
            {
                new WorkflowMap(new string[]{"TO_DO"}, "STUFF TO DO", StateType.ToDo, "TEST_TYPE", 1),
                new WorkflowMap(new string[]{"DONE"}, "FINISHED", StateType.Done, "TEST_TYPE", 4)
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "DONE"
            };

            var target = IssueTimeInStateMapper.GetInProgressDate(testIssue, partialWorkflowMap);
            Assert.AreEqual(new DateTime(2020, 10, 9), target);
        }

        [TestMethod]
        public void ShouldNotGetValidInProgressDateWhenNotStarted()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "TO_DO"
            };

            var target = IssueTimeInStateMapper.GetInProgressDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(DateTime.MinValue, target);
        }

        [TestMethod]
        public void ShouldGetLastDoneDate()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "DOING", "DONE"),
                new JiraState(new DateTime(2020, 10, 8), "DONE", "TO_DO"),
                new JiraState(new DateTime(2020, 10, 9), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 10), "DOING", "DONE")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "DONE"
            };

            var target = IssueTimeInStateMapper.GetDoneDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(new DateTime(2020, 10, 10), target);
        }

        [TestMethod]
        public void ShouldNotGetDoneDateWhenNotDone()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "", "TO_DO"),
                new JiraState(new DateTime(2020,10,8), "TO_DO", "DOING"),
                new JiraState(new DateTime(2020, 10, 9), "DOING", "VALIDATING")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "VALIDATING"
            };

            var target = IssueTimeInStateMapper.GetDoneDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(DateTime.MinValue, target);
        }

        [TestMethod]
        public void ShouldNotGetDoneDateWhenMovedBackFromDone()
        {
            var jiraStates = new List<JiraState>()
            {
                new JiraState(new DateTime(2020,10,7), "DOING", "DONE"),
                new JiraState(new DateTime(2020,10,8), "DONE", "DOING")
            };

            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                JiraStates = jiraStates,
                Type = "TEST_TYPE",
                Status = "DOING"
            };

            var target = IssueTimeInStateMapper.GetDoneDate(testIssue, _mockWorkflowMap);
            Assert.AreEqual(DateTime.MinValue, target);
        }

        [TestMethod]
        public void ShouldGeteCycleTimeForDoneIssue()
        {
            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                InProgressDate = DateTime.Today,
                DoneDate = DateTime.Today.AddDays(3)
            };

            var target = IssueTimeInStateMapper.GetCycleTime(testIssue);

            Assert.AreEqual(4, target);
        }

        [TestMethod]
        public void ShouldNotGetCycleTimeForIssueStartedButNotDone()
        {
            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION")
            {
                InProgressDate = DateTime.Today
            };

            var target = IssueTimeInStateMapper.GetCycleTime(testIssue);

            Assert.IsNull(target);
        }

        [TestMethod]
        public void ShouldNotGetCycleTimeForIssueNotStarted()
        {
            var testIssue = new JiraIssue("TEST_KEY", "DESCRIPTION");

            var target = IssueTimeInStateMapper.GetCycleTime(testIssue);

            Assert.IsNull(target);
        }
    }
}
