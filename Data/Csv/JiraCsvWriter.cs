using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;
using JiraWriter.Model;
using JiraWriter.Extension;

namespace JiraWriter.Data.Csv
{
    public static class JiraCsvWriter
    {
        public static void Write(IEnumerable<JiraIssue> issues, string fileNameAndPath)
        {
            var csvRecords = new List<dynamic>();

            issues.ToList().ForEach(issue =>
            {
                var fieldsToWrite = GetWritableFields(issue);

                csvRecords.Add(fieldsToWrite.BuildCsvObject());
            });

            using (var writer = new StreamWriter(fileNameAndPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(csvRecords);
            }
        }

        public static Dictionary<string, object> GetWritableFields(JiraIssue issue)
        {
            var fieldsToWrite = new Dictionary<string, object>();
            //TODO:make this dynamic based on config
            fieldsToWrite.Add("Key", issue.Key);
            fieldsToWrite.Add("Description", issue.Description);
            fieldsToWrite.Add("Type", issue.Type);
            fieldsToWrite.Add("Team", issue.Team);
            fieldsToWrite.Add("Status", issue.Status);
            fieldsToWrite.Add("Labels", string.Join(" | ", issue.Labels));
            fieldsToWrite.Add("InProgressDate", issue.InProgressDate.ToDisplayDate());
            fieldsToWrite.Add("DoneDate", issue.DoneDate.ToDisplayDate());

            var cycleTime = (!issue.DoneDate.Equals(DateTime.MinValue) && !issue.InProgressDate.Equals(DateTime.MinValue))
                    ? issue.InProgressDate.NumberOfDays(issue.DoneDate, new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }).ToString()
                    : string.Empty;

            fieldsToWrite.Add("CycleTime", cycleTime);

            foreach (var state in issue.TimeInStates)
            {
                fieldsToWrite.Add(state.StateName, state.Date.ToDisplayDate());
            }

            foreach(var state in issue.TimeInStates)
            {
                fieldsToWrite.Add($"{state.StateName} (days)", (state.DaysInState <= 0) ? "" : state.DaysInState.ToString());
            }

            return fieldsToWrite;
        }
    }
}
