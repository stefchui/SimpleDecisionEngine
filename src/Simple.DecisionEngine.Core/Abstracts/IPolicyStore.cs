namespace Simple.DecisionEngine.Core.Abstracts
{
    /// <summary>
    /// Abstraction for storing and loading learned policies
    /// </summary>
    public interface IPolicyStore
    {
        Task SaveAsync(
            string tenantId,
            string policyKey,
            int version,
            int[] policy,
            TimeSpan? ttl = null);

        Task<int[]> LoadAsync(
            string tenantId,
            string policyKey,
            int? version = null);
    }
}
