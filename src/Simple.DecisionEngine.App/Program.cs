using Microsoft.Extensions.DependencyInjection;
using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Engines;
using Simple.DecisionEngine.Core.Models;
using Simple.DecisionEngine.Infrastructure.Constraints;
using Simple.DecisionEngine.Infrastructure.DependencyInjection;
using Simple.DecisionEngine.Samples;
using System.Data;

namespace Simple.DecisionEngine.App
{
    internal class Program
    {
        // -----------------------------
        // Policy identity (IMPORTANT)
        // -----------------------------
        private const string TenantId = "tenantA";
        private const string PolicyKey = "warehouse_policy";
        private const int PolicyVersion = 1;
        static async Task Main(string[] args)
        {
            var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "run";

            // Build DI container
            var services = new ServiceCollection();

            // Register your existing decision engine stack (reward model, transition model, monte carlo, dp engine, etc.)
            // If you don't have AddDecisionEngine here, you can register manually.
            services.AddDecisionEngine(opt =>
            {
                opt.MonteCarloRuns = 1000;
                opt.MaxActionsPerTime = 1; // runtime constraint
                opt.DiscountFactor = 0.90;
                opt.RandomSeed = 12345;
            });

            // Choose ONE policy store implementation:
            // JSON (dev)
            services.AddPolicyStoreJson(basePath: "Data");

            var provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

            if (mode == "train")
            {
                await RunTrainingAsync(provider);
                return;
            }

            await RunRuntimeDecisionAsync(provider);
        }

        // -------------------------------
        // Mode 1: OFFLINE/ONLINE TRAINING
        // -------------------------------
        private static async Task RunTrainingAsync(IServiceProvider provider)
        {
            Console.WriteLine("=== TRAIN MODE (Policy Iteration) ===");
            var mdp = new WarehouseMdp();
            var policyEngine = new PolicyIterationEngine();

            var policy = policyEngine.SolveOptimalPolicy(mdp, gamma: 0.95);

            Console.WriteLine("Learned policy:");
            for (int s = 0; s < policy.Length; s++)
                Console.WriteLine($"State {s} -> Action {policy[s]}");

            var store = provider.GetRequiredService<IPolicyStore>();

            await store.SaveAsync(
                tenantId: TenantId,
                policyKey: PolicyKey,
                version: PolicyVersion,
                policy: policy,
                ttl: TimeSpan.FromDays(30));   // optional TTL

            Console.WriteLine("Policy saved via IPolicyStore");
        }

        // -------------------------
        // Mode 2: RUNTIME EXECUTION
        // -------------------------
        private static async Task RunRuntimeDecisionAsync(IServiceProvider provider)
        {
            Console.WriteLine("=== RUN MODE (DP Engine + Policy Bias) ===");
            // Load latest policy (or specific version) produced by training
            var store = provider.GetRequiredService<IPolicyStore>();

            var policy = await store.LoadAsync(
                tenantId: TenantId,
                policyKey: PolicyKey,
                version: PolicyVersion);

            Console.WriteLine("Loaded policy:");
            for (int i = 0; i < policy.Length; i++)
                Console.WriteLine($"State {i} -> Action {policy[i]}");

            var dpEngine = provider.GetRequiredService<DynamicProgrammingEngine>();


            // IMPORTANT:
            // Add an extra constraint that biases decisions toward learned policy.
            // This integrates PolicyIterationEngine -> DynamicProgrammingEngine
            var constraint = new PolicyBiasConstraint(policy);

            // Example runtime state (finite horizon short-term planning)
            int T = 5;
            int J = 2; // action0 / action1
            var initialState = new State(time: 0, jCount: J, horizon: T);

            var (value, dpPolicy) = dpEngine.Solve(initialState, alpha: 0.90, mcRuns: 500);

            Console.WriteLine($"DP optimal value (short horizon): {value:F4}");
            foreach (var t in dpPolicy.Keys.OrderBy(x => x))
            {
                Console.WriteLine($"t={t}: [{string.Join(",", dpPolicy[t].Actions)}]");
            }

            Console.WriteLine("Execute action at t=0:");
            Console.WriteLine($"Chosen: [{string.Join(",", dpPolicy[0].Actions)}]");

        }
    }
}