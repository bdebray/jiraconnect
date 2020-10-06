using System.Collections.Generic;
using JiraWriter.Model;

namespace JiraWriter.Data.Jira
{
    public interface IChangelogStore
    {
        List<JiraState> Get(string key);
    }
}