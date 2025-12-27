namespace Simple.DecisionEngine.Core.Models
{
    public sealed class RewardResult
    {
        public double Value { get; }

        public RewardResult(double value)
        {
            Value = value;
        }
    }
}