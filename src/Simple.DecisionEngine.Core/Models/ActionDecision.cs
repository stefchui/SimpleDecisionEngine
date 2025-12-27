namespace Simple.DecisionEngine.Core.Models
{
    public sealed class ActionDecision
    {
        public int[] Actions { get; } // 0/1 decisions per j

        public ActionDecision(int[] actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public override string ToString()
        {
            return "[" + string.Join(",", Actions.Select(a => a.ToString())) + "]";
        }
    }
}
