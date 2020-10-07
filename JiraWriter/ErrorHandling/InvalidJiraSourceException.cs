using System;
namespace JiraWriter.ErrorHandling
{
    public class InvalidJiraSourceException : Exception
    {
        public InvalidJiraSourceException(string message)
            : base(message)
        {
        }

        public InvalidJiraSourceException(string message, Exception baseException)
            : base(message, baseException)
        {
        }
    }
}
