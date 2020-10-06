using System;
using System.IO;
using System.Collections.Generic;
using JiraWriter.Config;
using Microsoft.Extensions.Configuration;
using System.Linq;
using JiraWriter.Data.Jira;

namespace JiraWriter
{
    class Program
    {
        static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            var config = configuration.GetSection("AppSettings").GetSection("JiraConfig").Get<JiraConfig>();
            var mappings = configuration.GetSection("AppSettings").GetSection("Mapping").Get<List<TeamMap>>();

            var enabledMappings = mappings.Where(map => map.Enabled).ToList();

            if (!enabledMappings.Any())
            {
                Console.WriteLine("There are no team mappings enabled");
                return;
            }

            Console.WriteLine($"Processing {enabledMappings.Count} team mappings...");

            enabledMappings.ForEach(map =>
            {
                var issueStore = new IssueStore(config);
                var changelogStore = new ChangelogStore(config);

                var issueWriter = new JiraStateWriter(map, issueStore, changelogStore);
                var issuesToWrite = issueWriter.GetIssues();

                Console.WriteLine($"Found {issuesToWrite.Count} issues for {map.TeamName}...");

                issueWriter.WriteIssues(issuesToWrite);

                Console.WriteLine($"Issues written to {map.OutputFileName}");
            });

            Console.WriteLine("Complete");
        }
    }
}
