using FluentAssertions;
using FluentAssertions.Execution;
using SharkE.Chip8.Threading;
using System;
using Xunit;

namespace SharkE.Chip8.Tests.Threading
{
    public class ThreadRunnerTest
    {
        private const double _frequencyValue = 250;
        private const double _subFrequencyValue = 50;

        private static readonly Frequency _frequency = new(_frequencyValue);
        private static readonly Frequency _subFrequency = _frequency.GetSubFrequency(_subFrequencyValue);

        private readonly FakeThread _thread1 = new(_frequency);
        private readonly FakeThread _thread2 = new(_subFrequency);

        [Fact]
        public void GivenRunner_WhenTick_ShouldIncrementClock()
        {
            var runner = new ThreadRunner(_frequency, Array.Empty<IThread>());
            var expectedTick = 5;
            var expectedClock = expectedTick * _frequency.Divider;

            runner.Tick(TimeSpan.FromSeconds(expectedTick / _frequency.Value));

            runner.Clock.Should().Be(expectedClock);
        }

        [Fact]
        public void GivenRunner_WhenTick_ShouldTickThreads()
        {
            var runner = new ThreadRunner(_frequency, new IThread[] { _thread1, _thread2 });
            var expectedTick1 = 6;
            var expectedTick2 = 2;
            var expectedClock1 = expectedTick1 * _frequency.Divider;
            var expectedClock2 = expectedTick2 * _subFrequency.Divider;

            runner.Tick(TimeSpan.FromSeconds(expectedTick1 / _frequency.Value));

            using (new AssertionScope())
            {
                _thread1.Clock.Should().Be(expectedClock1);
                _thread2.Clock.Should().Be(expectedClock2);
            }
        }

        [Fact]
        public void GivenRunner_WhenReset_ShouldResetThreads()
        {
            var runner = new ThreadRunner(_frequency, new IThread[] { _thread1, _thread2 });
            runner.Tick(TimeSpan.FromSeconds(5 / _frequency.Value));

            runner.Reset();

            using (new AssertionScope())
            {
                _thread1.Clock.Should().Be(0);
                _thread2.Clock.Should().Be(0);
            }
        }

        [Fact]
        public void GivenRunner_WhenNeedSynchronize_ShouldSynchronizeClocks()
        {
            var runner = new ThreadRunner(_frequency, new IThread[] { _thread1, _thread2 });

            runner.Tick(TimeSpan.FromSeconds(1));

            using (new AssertionScope())
            {
                _thread1.Clock.Should().Be(0);
                _thread2.Clock.Should().Be(0);
            }
        }
    }
}
