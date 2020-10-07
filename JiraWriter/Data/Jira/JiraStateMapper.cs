using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Model;
using JiraWriter.Extension;

namespace JiraWriter.Data.Jira
{
    public static class JiraStateMapper
    {
        public static List<JiraState> MapStates(List<JToken> changeLog)
        {
            var stateTransitions = new List<JiraState>();

            var str = changeLog.ToString();

            changeLog?.ForEach(logDay =>
            {
                stateTransitions.AddRange(logDay.GetMatchingToken("items").ToList().Where(item => item.GetMatchingToken("field").ToString().Equals("status")).Select(item =>
                    new JiraState(
                        DateTime.Parse(logDay.GetMatchingToken("created").ToString()),
                        item.GetMatchingToken("fromString").ToString() ?? string.Empty,
                        item.GetMatchingToken("toString").ToString() ?? string.Empty
                        )
                    ));
            });

            return stateTransitions.OrderBy(state => state.TransitionDate).ToList();
        }
    }
}
