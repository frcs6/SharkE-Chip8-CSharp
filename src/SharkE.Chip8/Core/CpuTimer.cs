using SharkE.Chip8.Threading;

namespace SharkE.Chip8.Core
{
    public class CpuTimer : BaseThread
    {
        public byte Value { get; set; }

        public CpuTimer(Frequency targetFrequency) :
            base(targetFrequency)
        {
        }

        protected override uint Execute()
        {
            if (Value > 0)
                --Value;

            return 1;
        }

        public override void Reset()
        {
            base.Reset();
            Value = 0;
        }
    }
}
