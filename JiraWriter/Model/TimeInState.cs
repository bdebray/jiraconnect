using System;

namespace JiraWriter.Model
{
    public class TimeInState
    {
        public string StateName { get; set; }
        public DateTime Date { get; set; }
        public int DaysInState { get; set; }
        public int Sequence { get; set; }

        public TimeInState (string state, DateTime date)
        {
            StateName = state;
            Date = date;
        }

        public TimeInState(string state, DateTime date, int sequence)
        {
            StateName = state;
            Date = date;
            Sequence = sequence;
        }
    }
}
