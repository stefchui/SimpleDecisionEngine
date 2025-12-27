using Simple.DecisionEngine.Core.Abstracts;

namespace Simple.DecisionEngine.Samples
{
    // <summary>
    /// 2-state warehouse:
    /// State 0 = normal
    /// State 1 = congested
    ///
    /// Action 0 = fast pick (good in normal)
    /// Action 1 = reorganize (best to escape congestion)
    /// </summary>
    public sealed class WarehouseMdp : IDiscreteMdp
    {
        public int StateCount => 2;
        public int ActionCount => 2;

        public IEnumerable<(int NextState, double Probability, double Reward)> Transitions(int state, int action)
        {
            // Normal state
            if (state == 0 && action == 0) yield return (0, 1.0, 5.0); // keep earning
            else if (state == 0 && action == 1) yield return (1, 1.0, 1.0); // reorganize causes short-term loss

            // Congested state
            else if (state == 1 && action == 0) yield return (1, 1.0, 0.0); // fast pick stuck
            else if (state == 1 && action == 1) yield return (0, 1.0, 2.0); // reorganize recovers
        }
    }
}