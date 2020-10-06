using System.Collections.Generic;
using System.Linq;
using JiraWriter.Config;
using JiraWriter.Model;
using JiraWriter.Data.Jira;
using JiraWriter.Data.Csv;

namespace JiraWriter
{
    public class JiraStateWriter
    {
        protected readonly IIssueStore _issueStore;
        protected readonly IChangelogStore _changelogStore;

        protected readonly TeamMap _teamMap;

        public JiraStateWriter(JiraConfig jiraConfig, TeamMap map)
        {
            _issueStore = new IssueStore(jiraConfig);
            _changelogStore = new ChangelogStore(jiraConfig);
            _teamMap = map;
        }

        public JiraStateWriter(TeamMap teamMap, IIssueStore issueStore, IChangelogStore changelogStore)
        {
            _teamMap = teamMap;
            _issueStore = issueStore;
            _changelogStore = changelogStore;
        }

        public void WriteIssues(List<JiraIssue> jiraIssues)
        {
            jiraIssues.ForEach(issue =>
            {
                if (issue.HasMoreChangeHistory)
                {
                    issue.JiraStates = _changelogStore.Get(issue.Key);
                }
                else
                {
                    issue.JiraStates = JiraStateMapper.MapStates(issue.RawChangelog.SelectToken("histories").ToList());
                }

                issue.Team = _teamMap.TeamName;

                issue.TimeInStates = IssueTimeInStateMapper.GetTimeInStates(issue, _teamMap.Workflow);
                issue.InProgressDate = IssueTimeInStateMapper.GetInProgressDate(issue, _teamMap.Workflow);
                issue.DoneDate = IssueTimeInStateMapper.GetDoneDate(issue, _teamMap.Workflow);
            });

            JiraCsvWriter.Write(jiraIssues, _teamMap.OutputFileName);
        }

        public void WriteIssues()
        {
            var jiraIssues = GetIssues();
            WriteIssues(jiraIssues);
        }

        public List<JiraIssue> GetIssues()
        {
            return _issueStore.Get(_teamMap.JiraQuery);
        }
    }
}
