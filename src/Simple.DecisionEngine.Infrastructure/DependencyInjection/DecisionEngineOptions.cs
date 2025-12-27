namespace Simple.DecisionEngine.Infrastructure.DependencyInjection
{
    // Simple options object for tuning
    public sealed class DecisionEngineOptions
    {
        public int MonteCarloRuns { get; set; } = 5000;
        public int MaxActionsPerTime { get; set; } = 2;
        public double DiscountFactor { get; set; } = 0.9;
        public int RandomSeed { get; set; } = 12345;
    }
}
