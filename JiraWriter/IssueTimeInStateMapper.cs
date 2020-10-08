using System;
using System.Collections.Generic;
using System.Linq;
using JiraWriter.Model;
using JiraWriter.Config;
using JiraWriter.Extension;

namespace JiraWriter
{
    public static class IssueTimeInStateMapper
    {
        /// <summary>
        /// Builds and returns a list of TimeInStates that include the states provided in the state map, their dates they entered each state, and the total duration spent in each state
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="stateMaps"></param>
        /// <param name="excludedDays"></param>
        /// <returns></returns>
        public static List<TimeInState> GetTimeInStates(JiraIssue issue, IEnumerable<WorkflowMap> stateMaps, List<DayOfWeek> excludedDays = null)
        {
            var timeInStates = new List<TimeInState>();
            var minDate = DateTime.MinValue;

            var issueTypeStateMaps = stateMaps.Where(map => map.IssueType.Equals(issue.Type)).OrderBy(map => map.Sequence).ToList();

            issueTypeStateMaps.ForEach(map =>
            {
                var state = (map.StateType.Equals(StateType.InProgress))
                    ? GetMinStateTransition(issue.JiraStates, map.JiraStates, ref minDate)
                    : GetMaxStateTransition(issue.JiraStates, map.JiraStates);

                var transitionDate = (state == null) ? DateTime.MinValue : state.TransitionDate;
                minDate = transitionDate;

                timeInStates.Add(new TimeInState(map.MappedState, transitionDate, map.Sequence));
            });

            //TODO:probably can do this more efficiently instead in a separate loop
            foreach (var timeInState in timeInStates)
            {
                var stateMap = stateMaps.Where(map => map.MappedState.Equals(timeInState.StateName)).FirstOrDefault();

                if (stateMap.StateType.Equals(StateType.Done)) continue;

                if (timeInState.Date == DateTime.MinValue) continue;

                var nextState = timeInStates.Where(state => state.Sequence > timeInState.Sequence && state.Date != DateTime.MinValue).OrderBy(state => state.Sequence).FirstOrDefault();

                var nextDate = (nextState == null) ? DateTime.Today : nextState.Date;

                var oldDaysInState = nextDate.Subtract(timeInState.Date).Days;
                var daysInState = timeInState.Date.NumberOfDays(nextDate, excludedDays);
                timeInState.DaysInState = daysInState;
            };

            return timeInStates;
        }
        
        /// <summary>
        /// Get the minimum transition date for statuses that are considered in progress
        /// Includes any transition to done if "In Progress" statuses were skipped
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="stateMaps"></param>
        /// <returns></returns>
        public static DateTime GetInProgressDate(JiraIssue issue, IEnumerable<WorkflowMap> stateMaps)
        {
            var inProgressStateMaps = stateMaps.Where(map => map.IssueType.Equals(issue.Type) && map.StateType.Equals(StateType.InProgress)).ToList();
            var toDoStateMaps = stateMaps.Where(map => map.IssueType.Equals(issue.Type) && map.StateType.Equals(StateType.ToDo)).ToList();

            if (inProgressStateMaps == null || !inProgressStateMaps.Any())
            {
                return GetDoneDate(issue, stateMaps);
            }

            var toDoStateNames = BuildStateNamesArray(toDoStateMaps);
            var inProgressStateNames = BuildStateNamesArray(inProgressStateMaps);

            var maxToDoState = GetMaxStateTransition(issue.JiraStates, toDoStateNames);

            var minimumDate = (maxToDoState == null) ? DateTime.MinValue : maxToDoState.TransitionDate;

            var minInProgressState = GetMinStateTransition(issue.JiraStates, inProgressStateNames, ref minimumDate);

            return (minInProgressState == null) ? GetDoneDate(issue, stateMaps) : minInProgressState.TransitionDate;
        }

        /// <summary>
        /// Get the max done date for all states that are labeled as the Done state type, unless the issue is no longer in a Done state
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="stateMaps"></param>
        /// <returns></returns>
        public static DateTime GetDoneDate(JiraIssue issue, IEnumerable<WorkflowMap> stateMaps)
        {
            var doneStateMapsForIssue = stateMaps.Where(map => map.IssueType.Equals(issue.Type) && map.StateType.Equals(StateType.Done)).ToList();
            var notDoneStateMapsForIssue = stateMaps.Where(map => map.IssueType.Equals(issue.Type) && !map.StateType.Equals(StateType.Done)).ToList();

            var doneStateNames = BuildStateNamesArray(doneStateMapsForIssue);

            if (!doneStateNames.Contains(issue.Status)) return DateTime.MinValue;

            var maxDoneState = GetMaxStateTransition(issue.JiraStates, doneStateNames);

            return (maxDoneState == null) ? DateTime.MinValue : maxDoneState.TransitionDate;
        }

        /// <summary>
        /// Returns the earliest transition to a state (or states if they both represent the same kind of transition on a team board)
        /// </summary>
        /// <param name="states"></param>
        /// <param name="stateNames"></param>
        /// <param name="minDate"></param>
        /// <returns></returns>
        private static JiraState GetMinStateTransition(IEnumerable<JiraState> states, string[] stateNames, ref DateTime minDate)
        {
            var tempMinDate = minDate == null ? new DateTime() : minDate;

            var stateMatches = states.Where(transition => stateNames.Contains(transition.ToState)).ToList();

            var state = stateMatches.Where(transition => transition.TransitionDate >= tempMinDate).OrderBy(transition => transition.TransitionDate).FirstOrDefault();

            minDate = (state == null) ? minDate : state.TransitionDate;

            return state;
        }

        /// <summary>
        /// Gets the latest transition to the given state (or states if they both represent the same kind of transition on a team board)
        /// </summary>
        /// <param name="states"></param>
        /// <param name="stateNames"></param>
        /// <returns></returns>
        private static JiraState GetMaxStateTransition(IEnumerable<JiraState> states, string[] stateNames)
        {
            return states.Where(transition => stateNames.Contains(transition.ToState)).OrderByDescending(transition => transition.TransitionDate).FirstOrDefault();
        }

        /// <summary>
        /// Builds and returns an array of unique state names for a list of workflow state maps
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        private static string[] BuildStateNamesArray(List<WorkflowMap> maps)
        {
            var stateNames = new string[] { };

            maps.ForEach(map =>
            {
                var tempArray = stateNames.Union(map.JiraStates).ToArray();

                stateNames = tempArray;
            });

            return stateNames;
        }
    }
}
