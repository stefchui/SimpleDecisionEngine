namespace Simple.DecisionEngine.Core.Models
{
    // Represents P(S' | S, d)
    public sealed class TransitionResult
    {
        public IReadOnlyList<(State NextState, double Probability)> Outcomes { get; }

        public TransitionResult(IReadOnlyList<(State, double)> outcomes)
        {
            Outcomes = outcomes;
        }
    }
}
