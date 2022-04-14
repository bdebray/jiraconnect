using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JiraWriter.Data.Jira
{
    public interface IChangelogStore
    {
        List<JToken> Get(string key);
    }
}