namespace SharkE.Chip8.Threading
{
    public interface IThread
    {
        double Clock { get; }

        void Tick();
        void Reset();
        void SynchronizeClock(double mainClock);
    }
}
