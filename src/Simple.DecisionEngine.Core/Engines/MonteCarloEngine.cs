using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Core.Engines
{
    public sealed class MonteCarloEngine
    {
        private readonly IRewardModel _rewardModel;

        public MonteCarloEngine(IRewardModel rewardModel)
        {
            _rewardModel = rewardModel ?? throw new ArgumentNullException(nameof(rewardModel));
        }

        public RewardResult Estimate(State state, ActionDecision decision, int runs)
        {
            double sum = 0;

            for (int i = 0; i < runs; i++)
                sum += _rewardModel.SampleReward(state, decision);

            return new RewardResult(sum / runs);
        }
    }
}
