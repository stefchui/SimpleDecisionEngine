using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Engines;
using Simple.DecisionEngine.Infrastructure.Constraints;
using Simple.DecisionEngine.Infrastructure.PolicyStore;
using Simple.DecisionEngine.Infrastructure.RewardModels;
using Simple.DecisionEngine.Infrastructure.TransitionModels;

namespace Simple.DecisionEngine.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDecisionEngine(
            this IServiceCollection services,
            Action<DecisionEngineOptions>? configure = null)
        {
            if (configure != null)
                services.Configure(configure);
            else
                services.AddOptions<DecisionEngineOptions>();

            // Reward model (example uses options)
            services.AddSingleton<IRewardModel>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<DecisionEngineOptions>>().Value;

                // Example means; replace with your real data source
                var means = new double[] { 10, 7, 5, 3 };

                return new StochasticRewardModel(
                    baseMeans: means,
                    noiseRange: 2.0,
                    seed: opt.RandomSeed);
            });

            // Constraints (register as many as you want)
            services.AddSingleton<IConstraint>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<DecisionEngineOptions>>().Value;
                return new MaxActionConstraint(opt.MaxActionsPerTime);
            });

            // Transition model (REQUIRED by DP engine)
            services.AddSingleton<ITransitionModel, MarkovTransitionModel>();
            // Constraints (0..N allowed)
            //services.AddSingleton<IConstraint>(new MaxActionConstraint(max: 2));

            services.AddSingleton<MonteCarloEngine>();
            services.AddSingleton<DynamicProgrammingEngine>();

            return services;
        }

        public static IServiceCollection AddPolicyStoreMongo(
            this IServiceCollection services,
            string connectionString,
            string databaseName)
        {
            services.AddSingleton<IMongoClient>(
                _ => new MongoClient(connectionString));

            services.AddSingleton<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(databaseName));

            services.AddSingleton<IPolicyStore, MongoPolicyStore>();

            return services;
        }

        public static IServiceCollection AddPolicyStoreJson(
           this IServiceCollection services,
           string basePath = "Data")
        {
            services.AddSingleton<IPolicyStore>(
                _ => new JsonPolicyStore(basePath));

            return services;
        }
    }
}