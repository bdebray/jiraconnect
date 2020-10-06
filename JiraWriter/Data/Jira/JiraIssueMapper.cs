using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Model;

namespace JiraWriter.Data.Jira
{
    public static class JiraIssueMapper
    {
        public static JiraIssue MapJiraIssue(JToken issueJson)
        {
            var fields = issueJson.SelectToken("fields").ToList();
            var issueSummary = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("summary")).FirstOrDefault().ToObject<JProperty>().Value.ToString();
            var labels = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("labels")).FirstOrDefault().ToObject<JProperty>().Value;
            var changeLog = issueJson.SelectToken("changelog");

            var jiraIssue = new JiraIssue(issueJson["key"].ToString(), issueSummary);

            jiraIssue.Type = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("issuetype")).FirstOrDefault().ToObject<JProperty>().Value["name"].ToString();
            jiraIssue.Status = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("status")).FirstOrDefault().ToObject<JProperty>().Value["name"].ToString();
            jiraIssue.Labels = labels.Values<string>().ToArray();
            jiraIssue.RawChangelog = changeLog;
            jiraIssue.HasMoreChangeHistory = changeLog.SelectToken("maxResults").Value<int>() < changeLog.SelectToken("total").Value<int>();

            return jiraIssue;
        }
    }
}
