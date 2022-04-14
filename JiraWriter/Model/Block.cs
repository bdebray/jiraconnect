using System;

namespace JiraWriter.Model
{
    public class Block
    {
        public Block(DateTime blockedDate)
        {
            BlockDate = blockedDate;
        }

        public Block(DateTime blockedDate, DateTime? unblockedDate)
        {
            BlockDate = blockedDate;
            UnblockDate = unblockedDate;
        }

        public Block() { }

        public DateTime BlockDate { get; set; }
        public DateTime? UnblockDate { get; set; } = null;
    }
}
