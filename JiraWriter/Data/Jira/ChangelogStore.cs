using System.Collections.Generic;
using System.Net.Http;
using JiraWriter.Config;
using Newtonsoft.Json.Linq;

namespace JiraWriter.Data.Jira
{
    public class ChangelogStore : JiraStore, IChangelogStore
    {
        public ChangelogStore(HttpClient client) : base(client)
        {
        }

        public ChangelogStore(JiraConfig config) : base(config)
        {
        }

        public List<JToken> Get(string key)
        {
            var endpointUri = $"{_config.BaseUrl}/issue/{key}/changelog";

            var options = new Dictionary<string, string>()
                {
                    { "fields", "values" }
                };

            var results = Get($"{endpointUri}", options, "values");

            return results;
        }
    }
}
