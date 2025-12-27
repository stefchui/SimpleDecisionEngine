using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Infrastructure.RewardModels
{
    // Samples reward for a decision at time t:
    // reward = sum_j ( (mean_j + noise) * d_j )
    // where noise is uniform in [-noiseRange, +noiseRange].
    public sealed class StochasticRewardModel : IRewardModel
    {
        private readonly double[] _means;
        private readonly double _noiseRange;
        private readonly Random _rng;

        public StochasticRewardModel(double[] baseMeans, double noiseRange, int seed = 12345)
        {
            _means = baseMeans ?? throw new ArgumentNullException(nameof(baseMeans));
            _noiseRange = noiseRange < 0 ? throw new ArgumentOutOfRangeException(nameof(noiseRange)) : noiseRange;
            _rng = new Random(seed);
        }

        public double SampleReward(State state, ActionDecision decision)
        {
            // Optional: make time matter (example: slight decay over time)
            double timeFactor = 1.0 - (0.03 * state.Time); // just an example
            if (timeFactor < 0.1) timeFactor = 0.1;

            double total = 0.0;

            for (int j = 0; j < state.JCount; j++)
            {
                if (decision.Actions[j] == 0)
                    continue;

                double noise = (_rng.NextDouble() * 2.0 - 1.0) * _noiseRange; // [-range, +range]
                double r = (_means[j] + noise) * timeFactor;

                total += r;
            }

            return total;
        }
    }
}
