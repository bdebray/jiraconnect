using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Model;
using JiraWriter.Extension;

namespace JiraWriter.Data.Jira
{
    public static class JiraBlockMapper
    {
        private const string TokenName = "Flagged";
        private const string BlockName = "Impediment";

        public static List<Block> Map(List<JToken> changeLog)
        {
            var blocks = new List<Block>();
            List<dynamic> flagChanges = new List<dynamic>();

            changeLog?.ForEach(logDay =>
            {
                var changeLogFlags = logDay.GetMatchingToken("items").ToList().Where(item => item.GetMatchingToken("field").ToString().Equals(TokenName)).Select(item =>
                new
                {
                    Date = DateTime.Parse(logDay.GetMatchingToken("created").ToString()),
                    Blocked = (item.GetMatchingToken("toString").ToString() ?? string.Empty).Equals(BlockName)
                }).OrderBy(item => item.Date).ToList();

                if (changeLogFlags.Any()) flagChanges.AddRange(changeLogFlags);
            });

            //the flag pattern should always be "Blocked" followed by an "Unblock". You cannot block or unblock items twice in a row
            flagChanges.OrderBy(change => (DateTime)change.Date).ToList().ForEach(change =>
            {
                if (change.Blocked)
                {
                    var block = new Block
                    {
                        BlockDate = change.Date
                    };

                    blocks.Add(block);
                }

                if (!change.Blocked)
                {
                    var finishedBlock = blocks.First(block => block.UnblockDate == null);
                    finishedBlock.UnblockDate = change.Date;
                }
            });

            return blocks.OrderBy(state => state.BlockDate).ToList();
        }
    }
}
