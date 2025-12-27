namespace Simple.DecisionEngine.Core.Abstracts
{
    /// <summary>
    /// Discrete Markov Decision Process definition
    /// </summary>
    public interface IDiscreteMdp
    {
        int StateCount { get; }
        int ActionCount { get; }

        IEnumerable<(int NextState, double Probability, double Reward)>
            Transitions(int state, int action);
    }
}
