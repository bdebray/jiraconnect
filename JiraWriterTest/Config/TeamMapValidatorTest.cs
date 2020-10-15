using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Config;
using JiraWriter.ErrorHandling;

namespace JiraWriterTest.Config
{
    [TestClass]
    public class TeamMapValidatorTest
    {
        [TestMethod]
        public void TeamMapShouldBeValid()
        {
            var validMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3)
                }
            };

            Assert.IsTrue(TeamMapValidator.IsValid(validMap));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithMultipleToDoStatesShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"ToDo2"}, "ANOTHER_TODO", StateType.ToDo, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 3),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 4)
                }
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithMultipleDoneStatesShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
            {
                new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3),
                new WorkflowMap(new string[]{"Done2"}, "DONE_AGAIN", StateType.Done, "test_issue_type", 4)
            }
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithDifferentMappedStateNamesAcrossIssueTypesShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3),
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "second_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DIFFERENT DOING", StateType.InProgress, "second_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "second_issue_type", 3),
                }
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithDifferentMappedStateCountsAcrossIssueTypesShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3),
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "second_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "second_issue_type", 2),
                    new WorkflowMap(new string[]{"Test"}, "VALIDATING", StateType.InProgress, "second_issue_type", 3),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "second_issue_type", 4),
                }
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithNoStatesShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithNoFileNameShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = string.Empty,
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3)
                }
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithInvalidFileExtensionShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = "project = 'TEST'",
                OutputFileName = "missing_the_csv",
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3)
                }
            };

            TeamMapValidator.IsValid(invalidMap);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMappingException))]
        public void TeamMapWithEmptyJiraQueryShouldNotBeValid()
        {
            var invalidMap = new TeamMap
            {
                JiraQuery = string.Empty,
                OutputFileName = "validname.csv",
                Workflow = new List<WorkflowMap>()
                {
                    new WorkflowMap(new string[]{"ToDo"}, "TODO", StateType.ToDo, "test_issue_type", 1),
                    new WorkflowMap(new string[]{"Doing"}, "DOING", StateType.InProgress, "test_issue_type", 2),
                    new WorkflowMap(new string[]{"Done"}, "DONE", StateType.Done, "test_issue_type", 3)
                }
            };

            TeamMapValidator.IsValid(invalidMap);
        }
    }
}
