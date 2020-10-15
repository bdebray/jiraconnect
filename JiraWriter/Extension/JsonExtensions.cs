using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JiraWriter.ErrorHandling;

namespace JiraWriter.Extension
{
    public static class JsonExtensions
    {
        public static JProperty GetMatchingProperty(this List<JToken> list, string match)
        {
            var matchingToken = list.Where(prop => prop.ToObject<JProperty>().Name.Equals(match)).FirstOrDefault();

            if (matchingToken == null) throw new MissingJiraFieldException($"Unable to map from Jira. Matching field, {match}, was not found in the results.");

            return matchingToken.ToObject<JProperty>();
        }

        public static JToken GetMatchingToken(this JToken json, string match)
        {
            try
            {
                return json.SelectToken(match, true);
            }
            catch (Exception exception)
            {
                throw new MissingJiraFieldException($"Jira property, {match}, not found for issue.", exception);
            }
        }
    }
}
