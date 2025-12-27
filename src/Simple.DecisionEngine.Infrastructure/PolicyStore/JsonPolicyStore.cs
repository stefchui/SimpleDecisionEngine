using Simple.DecisionEngine.Core.Abstracts;
using System.Text.Json;

namespace Simple.DecisionEngine.Infrastructure.PolicyStore
{
    public sealed class JsonPolicyStore : IPolicyStore
    {
        private readonly string _basePath;
        public JsonPolicyStore(string basePath = "data")
        {
            _basePath = basePath;
            Directory.CreateDirectory(_basePath);
        }
        public async Task SaveAsync(
            string tenantId,
            string policyKey,
            int version,
            int[] policy,
            TimeSpan? ttl = null)
        {

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("tenantId is required", nameof(tenantId));

            if (string.IsNullOrWhiteSpace(policyKey))
                throw new ArgumentException("policyKey is required", nameof(policyKey));

            if (policy == null || policy.Length == 0)
                throw new ArgumentException("policy cannot be null or empty", nameof(policy));

            var path = GetPath(tenantId, policyKey, version);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(
                policy,
                new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(path, json);
        }
        public async Task<int[]> LoadAsync(
            string tenantId,
            string policyKey,
            int? version = null)
        {
            if (!version.HasValue)
                throw new InvalidOperationException(
                    "JSON store requires explicit version");

            var path = GetPath(tenantId, policyKey, version.Value);
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<int[]>(json)!;
        }

        private string GetPath(string tenant, string key, int version)
            => Path.Combine(_basePath, tenant, $"{key}_v{version}.json");
    }
}