using FluentAssertions;
using SharkE.Chip8.Threading;
using Xunit;

namespace SharkE.Chip8.Tests.Threading
{
    public class BaseThreadTest
    {
        private static readonly Frequency _frequency = new(250, 4);
        private static readonly double _mainClock = 50;
        private readonly FakeThread _thread = new(_frequency);

        [Fact]
        public void GivenThread_WhenTick_ShouldExecute()
        {
            _thread.Tick();
            _thread.ExecuteCallCount.Should().Be(1);
        }

        [Fact]
        public void GivenThread_WhenTick_ShouldIncrementClock()
        {
            var expectedClock = _thread.ExecuteStep * _frequency.Divider;
            _thread.Tick();
            _thread.Clock.Should().Be(expectedClock);
        }

        [Fact]
        public void GivenThread_WhenReset_ShouldResetState()
        {
            _thread.Tick();
            _thread.Reset();
            _thread.Clock.Should().Be(0);
        }

        [Fact]
        public void GivenThread_WhenSynchronizeClock_ShouldAdjustClock()
        {
            _thread.Tick();
            var expectedClock = _thread.ExecuteStep * _frequency.Divider - _mainClock;

            _thread.SynchronizeClock(_mainClock);

            _thread.Clock.Should().Be(expectedClock);
        }
    }
}
