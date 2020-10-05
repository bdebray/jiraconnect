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

        public JToken RawChangelog { get; set; }

        public List<JiraState> JiraStates { get; set; } = new List<JiraState>();
        public List<TimeInState> TimeInStates { get; set; } = new List<TimeInState>();
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();

        public bool HasMoreChangeHistory => RawChangelog.SelectToken("maxResults").Value<int>() < RawChangelog.SelectToken("total").Value<int>();


        public JiraIssue(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}
