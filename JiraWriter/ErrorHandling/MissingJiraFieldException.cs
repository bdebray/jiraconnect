using System;

namespace JiraWriter.ErrorHandling
{
    public class MissingJiraFieldException : Exception
    {
        public MissingJiraFieldException(string message)
            : base(message)
        {
        }

        public MissingJiraFieldException(string message, Exception baseException)
            : base(message, baseException)
        {
        }
    }
}
