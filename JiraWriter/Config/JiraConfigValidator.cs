using JiraWriter.ErrorHandling;

namespace JiraWriter.Config
{
    public static class JiraConfigValidator
    {
        public static bool IsValid(JiraConfig config)
        {
            if (config.ApiKey.Length <= 0) throw new InvalidJiraConfigException("ApiKey cannot be empty.");
            if (config.BaseUrl.Length <= 0) throw new InvalidJiraConfigException("The BaseUrl cannot be empty.");

            return true;
        }
    }
}
