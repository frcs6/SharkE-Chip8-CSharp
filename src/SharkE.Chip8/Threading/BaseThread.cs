namespace SharkE.Chip8.Threading
{
    public abstract class BaseThread : IThread
    {
        protected readonly Frequency _targetFrequency;

        public double Clock { get; private set; }

        protected BaseThread(Frequency targetFrequency)
        {
            _targetFrequency = targetFrequency;
        }

        public virtual void Tick() => IncrementClock(Execute());

        public virtual void Reset()
        {
            Clock = 0;
        }

        public virtual void SynchronizeClock(double mainClock)
        {
            Clock -= mainClock;
        }

        protected abstract uint Execute();

        protected virtual void IncrementClock(uint tick)
        {
            Clock += tick * _targetFrequency.Divider;
        }
    }
}
