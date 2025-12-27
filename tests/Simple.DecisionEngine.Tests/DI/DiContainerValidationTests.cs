using Microsoft.Extensions.DependencyInjection;
using Simple.DecisionEngine.Core.Engines;
using Simple.DecisionEngine.Infrastructure.DependencyInjection;

namespace Simple.DecisionEngine.Tests.DI
{
    [TestClass]
    public class DiContainerValidationTests
    {
        [TestMethod]
        public void DI_Container_Should_Build_And_Resolve_Key_Services()
        {
            var services = new ServiceCollection();

            // Register your engine services
            services.AddDecisionEngine(opt =>
            {
                opt.MonteCarloRuns = 10;
                opt.MaxActionsPerTime = 1;
                opt.DiscountFactor = 0.9;
                opt.RandomSeed = 12345;
            });

            // Validate DI graph (throws if anything missing)
            var provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

            // Key resolves
            Assert.IsNotNull(provider.GetRequiredService<DynamicProgrammingEngine>());
        }
    }
}
