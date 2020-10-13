using System.Collections.Generic;
using JiraWriter.Model;
using JiraWriter.Extension;

namespace JiraWriter.Data.Csv
{
    public static class JiraIssueCsvMapper
    {
        public static Dictionary<string, object> GetCsvFields(JiraIssue issue)
        {
            var fieldsToWrite = new Dictionary<string, object>
            {
                { "Key", issue.Key },
                { "Description", issue.Description },
                { "Type", issue.Type },
                { "Team", issue.Team },
                { "Status", issue.Status },
                { "Labels", string.Join(" | ", issue.Labels) },
                { "InProgressDate", issue.InProgressDate.ToDisplayDate() },
                { "DoneDate", issue.DoneDate.ToDisplayDate() },
                { "CycleTime", issue.CycleTime?.ToString() ?? string.Empty }
            };

            foreach (var state in issue.TimeInStates)
            {
                fieldsToWrite.Add(state.StateName, state.Date.ToDisplayDate());
            }

            foreach (var state in issue.TimeInStates)
            {
                fieldsToWrite.Add($"{state.StateName} (days)", (state.DaysInState <= 0) ? "" : state.DaysInState.ToString());
            }

            return fieldsToWrite;
        }
    }
}
