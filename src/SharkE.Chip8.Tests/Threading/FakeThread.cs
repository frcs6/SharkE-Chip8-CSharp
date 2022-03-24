using SharkE.Chip8.Threading;

namespace SharkE.Chip8.Tests.Threading
{
    public class FakeThread : BaseThread
    {
        public int ExecuteCallCount { get; set; }
        public uint ExecuteStep { get; set; } = 2;

        public FakeThread(Frequency frequency) :
            base(frequency)
        {
        }

        protected override uint Execute()
        {
            ++ExecuteCallCount;
            return ExecuteStep;
        }
    }
}
