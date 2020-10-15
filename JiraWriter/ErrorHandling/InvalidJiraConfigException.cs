using System;
namespace JiraWriter.ErrorHandling
{
    public class InvalidJiraConfigException: Exception
    {
        public InvalidJiraConfigException(string message)
            : base(message)
        {
        }
    }
}
