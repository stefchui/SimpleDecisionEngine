using MongoDB.Driver;
using Simple.DecisionEngine.Core.Abstracts;

namespace Simple.DecisionEngine.Infrastructure.PolicyStore
{
    public sealed class MongoPolicyStore : IPolicyStore
    {
        private readonly IMongoCollection<PolicyDocument> _collection;
        public MongoPolicyStore(IMongoDatabase database)
        {
            _collection = database.GetCollection<PolicyDocument>("policies");

            // Ensure indexes
            var keys = Builders<PolicyDocument>.IndexKeys
                .Ascending(x => x.TenantId)
                .Ascending(x => x.PolicyKey)
                .Ascending(x => x.Version);

            _collection.Indexes.CreateOne(
                new CreateIndexModel<PolicyDocument>(keys));

            // TTL index (Mongo auto-expires)
            _collection.Indexes.CreateOne(
                new CreateIndexModel<PolicyDocument>(
                    Builders<PolicyDocument>.IndexKeys.Ascending(x => x.ExpireAt),
                    new CreateIndexOptions { ExpireAfter = TimeSpan.Zero }));
        }
        public async Task SaveAsync(
            string tenantId,
            string policyKey,
            int version,
            int[] policy,
            TimeSpan? ttl = null)
        {
            var doc = new PolicyDocument
            {
                TenantId = tenantId,
                PolicyKey = policyKey,
                Version = version,
                Actions = policy,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = ttl.HasValue
                    ? DateTime.UtcNow.Add(ttl.Value)
                    : null
            };

            await _collection.InsertOneAsync(doc);
        }
        public async Task<int[]> LoadAsync(
            string tenantId,
            string policyKey,
            int? version = null)
        {
            var filter = Builders<PolicyDocument>.Filter
                .Eq(x => x.TenantId, tenantId) &
                Builders<PolicyDocument>.Filter
                .Eq(x => x.PolicyKey, policyKey);

            if (version.HasValue)
            {
                filter &= Builders<PolicyDocument>.Filter
                    .Eq(x => x.Version, version.Value);
            }

            var doc = await _collection
                .Find(filter)
                .SortByDescending(x => x.Version)
                .FirstOrDefaultAsync();

            if (doc == null)
                throw new KeyNotFoundException(
                    $"Policy not found: {tenantId}/{policyKey}");

            return doc.Actions;
        }
    }
}