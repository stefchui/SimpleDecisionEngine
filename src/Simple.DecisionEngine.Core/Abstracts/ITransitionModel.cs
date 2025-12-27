using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Core.Abstracts
{
    public interface ITransitionModel
    {
        TransitionResult GetTransitions(State state, ActionDecision decision);
    }
}
