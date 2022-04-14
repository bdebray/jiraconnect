using System;
using System.Collections.Generic;
using JiraWriter.Config;
using JiraWriter.Model;
using JiraWriter.Data.Jira;
using JiraWriter.Data.Csv;

namespace JiraWriter
{
    public class IssueWriter
    {
        protected readonly IIssueStore _issueStore;
        protected readonly IChangelogStore _changelogStore;
        protected readonly List<DayOfWeek> _weekdaysToExclude;

        protected readonly TeamMap _teamMap;
        protected readonly int _blockDurationMinutesThreshold = 0;

        public IssueWriter(JiraConfig jiraConfig, TeamMap map)
                    : this(jiraConfig, map, new IssueStore(jiraConfig), new ChangelogStore(jiraConfig))
        {
        }

        public IssueWriter(JiraConfig jiraConfig, TeamMap teamMap, IIssueStore issueStore, IChangelogStore changelogStore, List<DayOfWeek> daysToExclude = null)
        {
            _teamMap = teamMap;
            _issueStore = issueStore;
            _changelogStore = changelogStore;
            _blockDurationMinutesThreshold = jiraConfig.BlockedDurationMinutesThreshold;
            _weekdaysToExclude = daysToExclude == null
                ? new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
                : _weekdaysToExclude = daysToExclude;
        }

        public void WriteIssues(List<JiraIssue> jiraIssues)
        {
            JiraCsvWriter.Write(jiraIssues, _teamMap.OutputFileName);
        }

        public void WriteIssues()
        {
            var jiraIssues = GetIssues();
            WriteIssues(jiraIssues);
        }

        public List<JiraIssue> GetIssues()
        {
            var issues = _issueStore.Get(_teamMap.JiraQuery);

            issues.ForEach(issue =>
            {
                issue.Team = _teamMap.TeamName;

                if (issue.HasMoreChangeHistory)
                {
                    var changeLog = _changelogStore.Get(issue.Key);

                    issue.JiraStates = JiraStateMapper.Map(changeLog);
                    issue.Blocks = JiraBlockMapper.Map(changeLog);
                }

                issue.TimeInStates = IssueTimeInStateMapper.GetTimeInStates(issue, _teamMap.Workflow, _weekdaysToExclude);
                issue.InProgressDate = IssueTimeInStateMapper.GetInProgressDate(issue, _teamMap.Workflow);
                issue.DoneDate = IssueTimeInStateMapper.GetDoneDate(issue, _teamMap.Workflow);
                issue.CycleTime = IssueTimeInStateMapper.GetCycleTime(issue, _weekdaysToExclude);
                issue.BlockedTime = IssueBlockedTimeMapper.GetBlockedTime(issue, _blockDurationMinutesThreshold, _weekdaysToExclude);
            });

            return issues;
        }
    }
}
