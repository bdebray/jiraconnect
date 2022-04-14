using System;
using System.Collections.Generic;
using System.Linq;
using JiraWriter.Model;
using JiraWriter.Extension;

namespace JiraWriter
{
    public static class IssueBlockedTimeMapper
    {
        public static int GetBlockedTime(JiraIssue issue, int blockDurationMinutesThreshold = 0, List<DayOfWeek> excludedDays = null)
        {
            int blockedTime = 0;

            if (issue.Blocks.Any())
            {
                issue.Blocks.ForEach(block =>
                {
                    if (!block.BlockDate.Equals(DateTime.MinValue) && block.UnblockDate != null && !block.UnblockDate.Equals(DateTime.MinValue)
                        && IsBlockDurationBeyondMinuteThreshold(block, blockDurationMinutesThreshold))
                    {
                        blockedTime += block.BlockDate.NumberOfDays((DateTime)block.UnblockDate, excludedDays);
                    }
                });
            }

            return blockedTime;
        }

        private static bool IsBlockDurationBeyondMinuteThreshold(Block block, int threshold)
        {
            if (threshold <= 0) return true;

            var durationMinutes = ((DateTime)block.UnblockDate - block.BlockDate).TotalMinutes;

            return durationMinutes >= threshold;
        }
    }
}
