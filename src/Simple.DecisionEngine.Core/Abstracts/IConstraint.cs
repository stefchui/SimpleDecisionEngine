using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Core.Abstracts
{
    public interface IConstraint
    {
        bool IsSatisfied(State state, ActionDecision decision);
        string Name { get; }
    }
}