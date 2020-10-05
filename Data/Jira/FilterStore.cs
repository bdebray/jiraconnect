using System.Collections.Generic;
using System.Net.Http;
using JiraWriter.Config;

namespace JiraWriter.Data.Jira
{
    public class FilterStore : JiraStore
    {
        public FilterStore(HttpClient client) : base(client)
        {
        }

        public FilterStore(JiraConfig config) : base(config)
        {
        }

        public string Get(string key)
        {
            var endpointUri = $"{_config.BaseUrl}/filter/search";

            var options = new Dictionary<string, string>()
                {
                    { "id", key },
                    { "fields", "jql" },
                    { "expand", "jql" }
                };

            var results = Get($"{endpointUri}", options, "values");

            return results[0].SelectToken("jql").ToString();
        }
    }
}
