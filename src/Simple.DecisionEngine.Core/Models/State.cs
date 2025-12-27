namespace Simple.DecisionEngine.Core.Models
{
    public sealed class State
    {
        public int Time { get; }
        public int JCount { get; }
        public int Horizon { get; }

        public bool IsTerminal => Time >= Horizon;

        public State(int time, int jCount, int horizon)
        {
            Time = time;
            JCount = jCount;
            Horizon = horizon;
        }

        public State Next()
        {
            return new State(Time + 1, JCount, Horizon);
        }
    }
}
