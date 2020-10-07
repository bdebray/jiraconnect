using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JiraWriter.Model
{
    public class JiraIssue
    {
        public string Key { get; private set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string[] Labels { get; set; }
        public string Team { get; set; }
        public DateTime InProgressDate { get; set; }
        public DateTime DoneDate { get; set; }
        public List<JiraState> JiraStates { get; set; } = new List<JiraState>();
        public List<TimeInState> TimeInStates { get; set; } = new List<TimeInState>();
        public bool HasMoreChangeHistory { get; set; }
        internal List<JToken> RawChangelogHistories { get; set; }

        public JiraIssue(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}
