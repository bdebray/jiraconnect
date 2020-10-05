using System.Collections.Generic;

namespace JiraWriter.Config
{
    public class TeamMap
    {
        public string TeamName { get; set; }
        public string OutputFileName { get; set; }  
        public string JiraQuery { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<WorkflowMap> Workflow { get; set; }

        public TeamMap()
        {
        }
    }
}
