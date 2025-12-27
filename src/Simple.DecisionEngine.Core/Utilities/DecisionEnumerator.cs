using Simple.DecisionEngine.Core.Models;

namespace Simple.DecisionEngine.Core.Utilities
{
    public static class DecisionEnumerator
    {
        // Enumerate all 0/1 vectors of length J (2^J decisions).
        public static IEnumerable<ActionDecision> EnumerateAll(int jCount)
        {
            int total = 1 << jCount;

            for (int mask = 0; mask < total; mask++)
            {
                int[] actions = new int[jCount];
                for (int j = 0; j < jCount; j++)
                {
                    actions[j] = (mask >> j) & 1;
                }
                yield return new ActionDecision(actions);
            }
        }
    }
}