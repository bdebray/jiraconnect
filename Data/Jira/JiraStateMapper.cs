using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Model;

namespace JiraWriter.Data.Jira
{
    public static class JiraStateMapper
    {
        public static List<JiraState> MapStates(List<JToken> changeLog)
        {
            if (changeLog is null)
            {
                throw new ArgumentNullException(nameof(changeLog));
            }

            var stateTransitions = new List<JiraState>();

            changeLog.ForEach(logItem =>
            {
                stateTransitions.AddRange(logItem.SelectToken("items").ToList().Where(item => item.SelectToken("field").ToString().Equals("status")).Select(item =>
                    new JiraState(
                        DateTime.Parse(logItem.SelectToken("created").ToString()),
                        item.SelectToken("fromString").ToString() ?? string.Empty,
                        item.SelectToken("toString").ToString() ?? string.Empty
                        )
                    ));
            });

            return stateTransitions.OrderBy(state => state.TransitionDate).ToList();
        }
    }
}
