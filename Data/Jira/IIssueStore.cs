using System.Collections.Generic;
using JiraWriter.Model;

namespace JiraWriter.Data.Jira
{
    public interface IIssueStore
    {
        List<JiraIssue> Get(string filter);
    }
}