using System;
namespace JiraWriter.ErrorHandling
{
    public class InvalidMappingException : Exception
    {
        public InvalidMappingException(string message)
            : base(message)
        {
        }
    }
}
