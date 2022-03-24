namespace SharkE.Chip8.Threading
{
    public struct Frequency
    {
        public double Value { get; }
        public double Divider { get; }

        public Frequency(double frequency, double divider = 1)
        {
            Divider = divider;
            Value = frequency;
        }

        public Frequency GetSubFrequency(double frequency, double divider = 1)
            => new(frequency, divider * Value / frequency);

    }
}
