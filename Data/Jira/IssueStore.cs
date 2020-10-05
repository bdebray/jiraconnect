using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using JiraWriter.Model;
using JiraWriter.Config;

namespace JiraWriter.Data.Jira
{
    public class IssueStore : JiraStore, IIssueStore
    {
        public IssueStore(HttpClient client) : base(client)
        {
        }

        public IssueStore(JiraConfig config) : base(config)
        {
        }

        public List<JiraIssue> Get(string filter)
        {
            var endpointUri = $"{_config.BaseUrl}/search";

            var options = new Dictionary<string, string>()
                {
                    {"jql", filter},
                    { "fields", "key,issuetype,summary,labels,status,customfield_10119,customfield_10005,customfield_10115,changelog" },
                    { "expand", "changelog"}
                };

            var issues = Get(endpointUri, options, "issues").Select(issue =>
            {
                return JiraIssueMapper.MapJiraIssue(issue);
            }).ToList();

            return issues;
        }
    }
}
