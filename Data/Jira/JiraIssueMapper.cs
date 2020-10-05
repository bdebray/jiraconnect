using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Model;

namespace JiraWriter.Data.Jira
{
    public static class JiraIssueMapper
    {
        public static JiraIssue MapJiraIssue(JToken jToken)
        {
            var fields = jToken.SelectToken("fields").ToList();
            var issueSummary = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("summary")).FirstOrDefault().ToObject<JProperty>().Value.ToString();

            var jiraIssue = new JiraIssue(jToken["key"].ToString(), issueSummary);

            fields.ForEach(field =>
            {
                var property = field.ToObject<JProperty>();
                jiraIssue.Fields.Add(property.Name, property.Value.ToString());
            });

            jiraIssue.Type = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("issuetype")).FirstOrDefault().ToObject<JProperty>().Value["name"].ToString();
            jiraIssue.Description = jiraIssue.Fields["summary"];
            jiraIssue.Status = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("status")).FirstOrDefault().ToObject<JProperty>().Value["name"].ToString();
            var labels = fields.Where(prop => prop.ToObject<JProperty>().Name.Equals("labels")).FirstOrDefault().ToObject<JProperty>().Value;
            jiraIssue.Labels = labels.Values<string>().ToArray();

            jiraIssue.RawChangelog = jToken.SelectToken("changelog");

            return jiraIssue;
        }
    }
}
