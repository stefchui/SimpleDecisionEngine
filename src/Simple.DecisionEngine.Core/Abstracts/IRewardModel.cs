using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Core.Abstracts
{
    public interface IRewardModel
    {
        // Sample a stochastic reward for a given (state, decision).
        // This corresponds to sampling R_j(t) terms and combining with d_j(t).
        double SampleReward(State state, ActionDecision decision);
    }
}
