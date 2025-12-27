using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Infrastructure.Constraints
{
    /// <summary>
    /// "Soft-ish" constraint: prefer recommended action from learned policy,
    /// but still allow any 1-action decision (fallback).
    /// </summary>
    public sealed class PolicyBiasConstraint : IConstraint
    {
        public string Name => "PolicyBias";

        private readonly int[] _policy; // policy[stateIndex] = recommended action index

        public PolicyBiasConstraint(int[] policy)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            if (_policy.Length == 0) throw new ArgumentException("Policy cannot be empty.", nameof(policy));
        }

        public bool IsSatisfied(State state, ActionDecision decision)
        {
            // if decision selects more than 1 action, reject (keep it simple)
            if (decision.Actions.Sum() != 1) return false;

            int stateIndex = Math.Clamp(state.Time, 0, _policy.Length - 1);
            int recommendedAction = _policy[stateIndex];

            // allow recommended, OR allow any single action as fallback
            return recommendedAction >= 0 &&
                   recommendedAction < decision.Actions.Length &&
                   (decision.Actions[recommendedAction] == 1 || decision.Actions.Sum() == 1);
        }
    }
}
