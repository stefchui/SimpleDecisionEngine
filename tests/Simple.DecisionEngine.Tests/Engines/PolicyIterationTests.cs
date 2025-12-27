using Moq;
using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Engines;

namespace Simple.DecisionEngine.Tests.Engines
{
    [TestClass]
    public class PolicyIterationTests
    {
        [TestMethod]
        public void PolicyIteration_Finds_Expected_Optimal_Policy_Using_Mock()
        {
            // ------------------------------------------------
            // Arrange
            // ------------------------------------------------

            var mdpMock = new Mock<IDiscreteMdp>();

            mdpMock.Setup(m => m.StateCount).Returns(2);
            mdpMock.Setup(m => m.ActionCount).Returns(2);

            // Define transitions via Moq
            mdpMock
                .Setup(m => m.Transitions(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((state, action) =>
                {
                    // State 0
                    if (state == 0 && action == 0)
                        return new List<(int, double, double)>
                        {
                            (0, 1.0, 5.0)
                        };

                    if (state == 0 && action == 1)
                        return new List<(int, double, double)>
                        {
                            (1, 1.0, 1.0)
                        };

                    // State 1
                    if (state == 1 && action == 0)
                        return new List<(int, double, double)>
                        {
                            (1, 1.0, 0.0)
                        };

                    if (state == 1 && action == 1)
                        return new List<(int, double, double)>
                        {
                            (0, 1.0, 2.0)
                        };

                    return new List<(int, double, double)>();
                });

            var engine = new PolicyIterationEngine();

            // ------------------------------------------------
            // Act
            // ------------------------------------------------

            var policy = engine.SolveOptimalPolicy(
                mdpMock.Object,
                gamma: 0.9);

            // ------------------------------------------------
            // Assert
            // ------------------------------------------------

            // Expected:
            // State 0 -> Action 0
            // State 1 -> Action 1
            Assert.AreEqual(0, policy[0]);
            Assert.AreEqual(1, policy[1]);

            // Optional: verify MDP interactions
            mdpMock.Verify(m => m.Transitions(It.IsAny<int>(), It.IsAny<int>()),
                           Times.AtLeastOnce);
        }
    }
}