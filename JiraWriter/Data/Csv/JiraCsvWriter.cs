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
                var fieldsToWrite = JiraIssueCsvMapper.GetCsvFields(issue);

                csvRecords.Add(fieldsToWrite.BuildCsvObject());
            });

            using (var writer = new StreamWriter(fileNameAndPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(csvRecords);
            }
        }
    }
}
