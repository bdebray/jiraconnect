namespace JiraWriter.Config
{
    public enum StateType { ToDo, InProgress, Done }

    public class WorkflowMap
    {
        public string[] JiraStates { get; set; }
        public string MappedState { get; set; }
        public StateType StateType { get; set; }
        public string IssueType { get; set; }
        public int Sequence { get; set; }

        public WorkflowMap()
        {
        }

        public WorkflowMap(string[] jiraState, string mappedState, StateType type, string issueType)
        {
            JiraStates = jiraState;
            MappedState = mappedState;
            StateType = type;
            IssueType = issueType;
        }
    }
}