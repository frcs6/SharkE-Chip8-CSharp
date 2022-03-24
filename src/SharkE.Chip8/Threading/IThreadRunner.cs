namespace SharkE.Chip8.Threading
{
    public interface IThreadRunner
    {
        double Clock { get; }

        void Tick(TimeSpan elapsed);
        void Reset();
    }
}
