using System;

namespace JiraWriter.Model
{
    public class JiraState
    {
        public DateTime TransitionDate { get; set; }
        public string FromState { get; set; }
        public string ToState { get; set; }

        public JiraState(DateTime transitionDate, string fromState, string toState)
        {
            TransitionDate = transitionDate;
            FromState = fromState;
            ToState = toState;
        }
    }
}
