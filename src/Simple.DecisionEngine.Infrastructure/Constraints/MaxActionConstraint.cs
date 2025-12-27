using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Infrastructure.Constraints
{
    public sealed class MaxActionConstraint : IConstraint
    {
        public string Name => "MaxAction";

        private readonly int _max;

        public MaxActionConstraint(int max)
        {
            _max = max;
        }

        public bool IsSatisfied(State state, ActionDecision decision)
        {
            int count = decision.Actions.Sum();
            return count <= _max;
        }
    }
}
