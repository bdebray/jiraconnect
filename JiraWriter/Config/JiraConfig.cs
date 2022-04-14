namespace JiraWriter.Config
{
    public class JiraConfig
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int BlockedDurationMinutesThreshold { get; set; }

        public JiraConfig()
        {
        }
    }
}
