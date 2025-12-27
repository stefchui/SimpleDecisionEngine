using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Models;
using Simple.DecisionEngine.Core.Utilities;

namespace Simple.DecisionEngine.Core.Engines
{
    public sealed class DynamicProgrammingEngine
    {
        private readonly MonteCarloEngine _mc;
        private readonly ITransitionModel _transitionModel;
        private readonly IEnumerable<IConstraint> _constraints;

        public DynamicProgrammingEngine(
            MonteCarloEngine mc,
            ITransitionModel transitionModel,
            IEnumerable<IConstraint> constraints)
        {
            _mc = mc;
            _transitionModel = transitionModel;
            _constraints = constraints;
        }

        public (double Value, Dictionary<int, ActionDecision> Policy)
            Solve(State initialState, double alpha, int mcRuns)
        {

            if (alpha <= 0 || alpha > 1.0)
                throw new ArgumentOutOfRangeException(nameof(alpha), "alpha must be in (0, 1].");

            int T = initialState.Horizon;
            int J = initialState.JCount;

            // V[t] = optimal value from time t to end
            var V = new double[T + 1];
            var policy = new Dictionary<int, ActionDecision>();

            // Terminal condition
            V[T] = 0;

            // BACKWARD DP (Bellman)
            for (int t = T - 1; t >= 0; t--)
            {
                var state = new State(t, J, T);
                double bestValue = double.NegativeInfinity;
                ActionDecision bestDecision = null;

                foreach (var d in DecisionEnumerator.EnumerateAll(J))
                {
                    if (_constraints.Any(c => !c.IsSatisfied(state, d)))
                        continue;

                    // E[ Σ_j R_j(t) d_j(t) | S(t) ]
                    var reward = _mc.Estimate(state, d, mcRuns).Value;

                    // α * E[V(S_{t+1})]
                    double future = 0;
                    var transitions = _transitionModel.GetTransitions(state, d);

                    foreach (var (nextState, prob) in transitions.Outcomes)
                        future += prob * V[nextState.Time];

                    double q = reward + alpha * future;

                    if (q > bestValue)
                    {
                        bestValue = q;
                        bestDecision = d;
                    }
                }

                V[t] = bestValue;
                policy[t] = bestDecision;
            }

            return (V[0], policy);
        }
    }
}