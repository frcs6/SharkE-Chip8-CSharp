using FluentAssertions;
using FluentAssertions.Execution;
using SharkE.Chip8.Threading;
using Xunit;

namespace SharkE.Chip8.Tests.Threading
{
    public class FrequencyTest
    {
        [Fact]
        public void GivenParameters_WhenCtor_ShouldSetProperties()
        {
            var frequency = new Frequency(200, 2);

            using (new AssertionScope())
            {
                frequency.Divider.Should().Be(2);
                frequency.Value.Should().Be(200);
            }
        }

        [Fact]
        public void GivenFrequency_WhenGetSubFrequency_ShouldComputeDivider()
        {
            var frequency = new Frequency(200, 2);
            
            var subFrequency = frequency.GetSubFrequency(50);

            using (new AssertionScope())
            {
                subFrequency.Divider.Should().Be(4);
                subFrequency.Value.Should().Be(50);
            }
        }
    }
}
