using Simple.DecisionEngine.Core.Models;
using Simple.DecisionEngine.Core.Abstracts;

namespace Simple.DecisionEngine.Infrastructure.TransitionModels
{
    // Simple deterministic time-advance transition
    public sealed class MarkovTransitionModel : ITransitionModel
    {
        public TransitionResult GetTransitions(State state, ActionDecision decision)
        {
            var next = new State(
                time: state.Time + 1,
                jCount: state.JCount,
                horizon: state.Horizon
            );

            return new TransitionResult(
                new List<(State, double)>
                {
                    (next, 1.0)
                });
        }
    }
}