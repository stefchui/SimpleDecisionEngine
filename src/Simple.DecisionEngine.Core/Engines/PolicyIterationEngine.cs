using Simple.DecisionEngine.Core.Abstracts;

namespace Simple.DecisionEngine.Core.Engines
{
    /// <summary>
    /// Policy Iteration algorithm for finite discrete MDPs
    /// </summary>
    public sealed class PolicyIterationEngine
    {
        public int[] SolveOptimalPolicy(
            IDiscreteMdp mdp,
            double gamma,
            int maxIterations = 1000,
            double evalTolerance = 1e-9)
        {
            if (mdp == null) throw new ArgumentNullException(nameof(mdp));
            if (gamma <= 0 || gamma > 1)
                throw new ArgumentOutOfRangeException(nameof(gamma));

            int S = mdp.StateCount;
            int A = mdp.ActionCount;

            // Initial policy: choose action 0 in all states
            var policy = Enumerable.Repeat(0, S).ToArray();
            var V = new double[S];

            for (int iter = 0; iter < maxIterations; iter++)
            {
                // 1) Policy Evaluation
                PolicyEvaluate(mdp, policy, V, gamma, evalTolerance);

                // 2) Policy Improvement
                bool stable = true;

                for (int s = 0; s < S; s++)
                {
                    int oldAction = policy[s];
                    int bestAction = ArgMaxAction(mdp, V, s, gamma, A);

                    policy[s] = bestAction;
                    if (bestAction != oldAction)
                        stable = false;
                }

                if (stable)
                    break;
            }

            return policy;
        }

        private static void PolicyEvaluate(
            IDiscreteMdp mdp,
            int[] policy,
            double[] V,
            double gamma,
            double tolerance)
        {
            int S = mdp.StateCount;

            while (true)
            {
                double delta = 0.0;

                for (int s = 0; s < S; s++)
                {
                    int action = policy[s];
                    double vNew = 0.0;

                    foreach (var (nextState, prob, reward) in mdp.Transitions(s, action))
                    {
                        vNew += prob * (reward + gamma * V[nextState]);
                    }

                    delta = Math.Max(delta, Math.Abs(vNew - V[s]));
                    V[s] = vNew;
                }

                if (delta < tolerance)
                    break;
            }
        }

        private static int ArgMaxAction(
            IDiscreteMdp mdp,
            double[] V,
            int state,
            double gamma,
            int actionCount)
        {
            int bestAction = 0;
            double bestQ = double.NegativeInfinity;

            for (int action = 0; action < actionCount; action++)
            {
                double q = 0.0;

                foreach (var (nextState, prob, reward) in mdp.Transitions(state, action))
                {
                    q += prob * (reward + gamma * V[nextState]);
                }

                if (q > bestQ)
                {
                    bestQ = q;
                    bestAction = action;
                }
            }

            return bestAction;
        }
    }
}
