using System;
using System.IO;
using System.Collections.Generic;
using JiraWriter.Config;
using Microsoft.Extensions.Configuration;
using System.Linq;
using JiraWriter.Data.Jira;
using JiraWriter.ErrorHandling;

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

            try
            {
                var config = configuration.GetSection("AppSettings").GetSection("JiraConfig").Get<JiraConfig>();

                if (!JiraConfigValidator.IsValid(config)) throw new Exception("Invalid configuration.");

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
                    try
                    {
                        if (!TeamMapValidator.IsValid(map)) throw new Exception("Invalid map.");

                        var issueStore = new IssueStore(config);
                        var changelogStore = new ChangelogStore(config);

                        var issueWriter = new JiraStateWriter(map, issueStore, changelogStore);
                        var issuesToWrite = issueWriter.GetIssues();

                        Console.WriteLine($"Found {issuesToWrite.Count} issues for {map.TeamName}...");

                        issueWriter.WriteIssues(issuesToWrite);

                        Console.WriteLine($"Issues written to {map.OutputFileName}");
                    }
                    catch(InvalidMappingException exception)
                    {
                        Console.WriteLine($"{exception.Message}. Continuing to next map...");
                    }
                });

                Console.WriteLine("Complete");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to process. {exception.Message}");
            }
        }
    }
}