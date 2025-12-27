using Moq;
using Simple.DecisionEngine.Core.Abstracts;
using Simple.DecisionEngine.Core.Engines;
using Simple.DecisionEngine.Core.Models;
using Simple.DecisionEngine.Infrastructure.Constraints;

namespace Simple.DecisionEngine.Tests.Engines
{
    [TestClass]
    public class DynamicProgrammingEngineTests
    {
        [TestMethod]
        public void DynamicProgrammingEngine_Uses_Mocked_Reward_And_Transition()
        {
            // ------------------------------------------------
            // Arrange
            // ------------------------------------------------

            // Mock IRewardModel
            var rewardMock = new Mock<IRewardModel>();

            // Reward logic:
            // Action [1,0] => 10
            // Action [0,1] => 1
            rewardMock
                .Setup(r => r.SampleReward(It.IsAny<State>(), It.IsAny<ActionDecision>()))
                .Returns<State, ActionDecision>((state, decision) =>
                {
                    if (decision.Actions.SequenceEqual(new[] { 1, 0 }))
                        return 10.0;
                    if (decision.Actions.SequenceEqual(new[] { 0, 1 }))
                        return 1.0;

                    return 0.0;
                });

            var mcEngine = new MonteCarloEngine(rewardMock.Object);

            // Mock ITransitionModel
            var transitionMock = new Mock<ITransitionModel>();

            transitionMock
                .Setup(t => t.GetTransitions(It.IsAny<State>(), It.IsAny<ActionDecision>()))
                .Returns<State, ActionDecision>((state, decision) =>
                {
                    var nextState = new State(
                        time: state.Time + 1,
                        jCount: state.JCount,
                        horizon: state.Horizon);

                    return new TransitionResult(
                        new List<(State, double)>
                        {
                            (nextState, 1.0)
                        });
                });

            // Constraint: max 1 action
            var constraints = new[] { new MaxActionConstraint(1) };

            var engine = new DynamicProgrammingEngine(
                mcEngine,
                transitionMock.Object,
                constraints);

            var initialState = new State(time: 0, jCount: 2, horizon: 1);

            // ------------------------------------------------
            // Act
            // ------------------------------------------------
            var (value, policy) = engine.Solve(
                initialState,
                alpha: 1.0,
                mcRuns: 1);

            // ------------------------------------------------
            // Assert
            // ------------------------------------------------

            // Optimal policy must choose action [1,0]
            CollectionAssert.AreEqual(
                new[] { 1, 0 },
                policy[0].Actions);

            // Optimal value must be 10
            Assert.AreEqual(10.0, value, 0.00001);

            // Verify reward model used
            rewardMock.Verify(
                r => r.SampleReward(It.IsAny<State>(), It.IsAny<ActionDecision>()),
                Times.AtLeastOnce);

            // Verify transition model used
            transitionMock.Verify(
                t => t.GetTransitions(It.IsAny<State>(), It.IsAny<ActionDecision>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public void DynamicProgrammingEngine_Respects_Discount_Factor_With_Mocked_Reward()
        {
            // ------------------------------------------------
            // Arrange
            // ------------------------------------------------

            // Reward always returns 10
            var rewardMock = new Mock<IRewardModel>();
            rewardMock
                .Setup(r => r.SampleReward(It.IsAny<State>(), It.IsAny<ActionDecision>()))
                .Returns(10.0);

            var mcEngine = new MonteCarloEngine(rewardMock.Object);

            var transitionMock = new Mock<ITransitionModel>();
            transitionMock
                .Setup(t => t.GetTransitions(It.IsAny<State>(), It.IsAny<ActionDecision>()))
                .Returns<State, ActionDecision>((state, decision) =>
                {
                    var next = new State(
                        state.Time + 1,
                        state.JCount,
                        state.Horizon);

                    return new TransitionResult(
                        new List<(State, double)> { (next, 1.0) });
                });

            var engine = new DynamicProgrammingEngine(
                mcEngine,
                transitionMock.Object,
                Enumerable.Empty<IConstraint>());

            var initialState = new State(time: 0, jCount: 1, horizon: 2);

            // ------------------------------------------------
            // Act
            // ------------------------------------------------
            var (value, _) = engine.Solve(
                initialState,
                alpha: 0.5,
                mcRuns: 1);

            // ------------------------------------------------
            // Assert
            // ------------------------------------------------

            // t=0: 10 + 0.5 * 10 = 15
            Assert.AreEqual(15.0, value, 0.00001);

            rewardMock.Verify(
                r => r.SampleReward(It.IsAny<State>(), It.IsAny<ActionDecision>()),
                Times.AtLeastOnce);
        }
    }
}