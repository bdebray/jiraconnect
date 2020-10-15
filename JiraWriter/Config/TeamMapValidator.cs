using System.Collections.Generic;
using System.Linq;
using JiraWriter.ErrorHandling;

namespace JiraWriter.Config
{
    public static class TeamMapValidator
    {
        public static bool IsValid(TeamMap map)
        {
            if (!MapHasProperCsvPathAndName(map)) throw new InvalidMappingException($"The 'OutputFileName' for TeamName {map.TeamName} is invalid. Ensure that the map has a proper path and filename (including '.csv').");
            if (!MapHasWorkflowStates(map)) throw new InvalidMappingException($"The workflow map for TeamName {map.TeamName} is empty.");
            if (!MapHasJiraQuery(map)) throw new InvalidMappingException($"The 'JiraQuery' for TeamName {map.TeamName} is empty.");

            ValidateCommonMappedStates(map);
            ValidateSingleStateTypeMapping(map, StateType.ToDo);
            ValidateSingleStateTypeMapping(map, StateType.Done);

            return true;
        }

        private static void ValidateCommonMappedStates(TeamMap map)
        {
            var firstSetOfMappedStateValues = new string[] { };

            var issueTypesInMap = map.Workflow.Select(state => state.IssueType).Distinct().ToList();

            issueTypesInMap.ForEach(issueType =>
            {
                var statesForIssueType = map.Workflow.Where(state => state.IssueType.Equals(issueType)).ToList();
                var mappedStates = statesForIssueType.Select(state => state.MappedState).Distinct().ToArray();

                if (!firstSetOfMappedStateValues.Any())
                {
                    firstSetOfMappedStateValues = mappedStates;
                }

                if (!mappedStates.SequenceEqual(firstSetOfMappedStateValues))
                {
                    throw new InvalidMappingException($"The workflow mapped for TeamName {map.TeamName} is invalid. MappedStates for {issueType} does not match the MappedStates of other issue types.");
                }
            });

        }

        private static void ValidateSingleStateTypeMapping(TeamMap map, StateType type)
        {
            var issueTypesInMap = map.Workflow.Select(state => state.IssueType).Distinct().ToList();

            issueTypesInMap.ForEach(issueType =>
            {
                if (GetMatchingStateTypes(map, issueType, type).Count() > 1)
                {
                    throw new InvalidMappingException($"The workflow mapped for TeamName {map.TeamName} is invalid. There should only be a single {type} state configured for each issue type.");
                }
            });
        }

        private static List<WorkflowMap> GetMatchingStateTypes(TeamMap map, string issueType, StateType type)
        {
            return map.Workflow.Where(state => state.IssueType.Equals(issueType) && state.StateType.Equals(type)).ToList();
        }

        private static bool MapHasProperCsvPathAndName(TeamMap map)
        {
            return !map.OutputFileName.Equals(string.Empty) && map.OutputFileName.EndsWith(".csv");
        }

        private static bool MapHasWorkflowStates(TeamMap map)
        {
            return map.Workflow.Count() > 0;
        }

        private static bool MapHasJiraQuery(TeamMap map)
        {
            return map.JiraQuery.Length > 0;
        }
    }
}
