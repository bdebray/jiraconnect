using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Model;
using JiraWriter.ErrorHandling;
using JiraWriter.Extension;

namespace JiraWriter.Data.Jira
{
    public static class JiraIssueMapper
    {
        public static JiraIssue MapJiraIssue(JToken issueJson)
        {
            if (!issueJson.Children().Any()) throw new InvalidJiraSourceException($"The source data from Jira is an invalid JSON object.");

            var fields = issueJson.GetMatchingToken("fields").ToList();
            var issueKey = issueJson.GetMatchingToken("key").Value<string>();
            var changeLog = issueJson.GetMatchingToken("changelog");
            var issueSummary = fields.GetMatchingProperty("summary").Value.ToString();
            var labels = fields.GetMatchingProperty("labels").Value;

            var jiraIssue = new JiraIssue(issueKey, issueSummary)
            {
                Type = fields.GetMatchingProperty("issuetype").Value["name"].ToString(),
                Status = fields.GetMatchingProperty("status").Value["name"].ToString(),
                Labels = labels.Values<string>().ToArray(),

                HasMoreChangeHistory = changeLog == null || changeLog.GetMatchingToken("maxResults").Value<int>() < changeLog.GetMatchingToken("total").Value<int>()
            };

            if (!jiraIssue.HasMoreChangeHistory)
            {
                jiraIssue.JiraStates = JiraStateMapper.MapStates(changeLog?.GetMatchingToken("histories").ToList());
            }

            return jiraIssue;
        }
    }
}
