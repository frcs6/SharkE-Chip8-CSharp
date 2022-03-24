namespace SharkE.Chip8.Threading
{
    public class ThreadRunner : IThreadRunner
    {
        private static readonly TimeSpan _synchronizeTime = TimeSpan.FromSeconds(1);

        private TimeSpan _elapsedTime;
        private double _incompleteTickToExecute;
        private readonly List<IThread> _threads;
        private readonly Frequency _targetFrequency;

        public double Clock { get; private set; }

        public ThreadRunner(Frequency targetFrequency, IEnumerable<IThread> threads)
        {
            _targetFrequency = targetFrequency;
            _threads = threads.ToList();
        }

        public void Tick(TimeSpan elapsed)
        {
            _incompleteTickToExecute += _targetFrequency.Value * elapsed.TotalSeconds;
            var completeTickToExecute = (int)Math.Truncate(_incompleteTickToExecute);
            _incompleteTickToExecute -= completeTickToExecute;

            for (var i = 0; i < completeTickToExecute; ++i)
            {
                var nextClock = Clock + _targetFrequency.Divider;

                double latestProcessorClock;
                do
                {
                    latestProcessorClock = double.MaxValue;
                    foreach (var processor in _threads)
                    {
                        if (processor.Clock < nextClock)
                            processor.Tick();

                        latestProcessorClock = Math.Min(latestProcessorClock, processor.Clock);
                    }
                } while (latestProcessorClock < nextClock);

                Clock = nextClock;
            }

            _elapsedTime += elapsed;
            if (_elapsedTime < _synchronizeTime)
                return;

            _elapsedTime -= _synchronizeTime;

            _threads.ForEach(p => p.SynchronizeClock(Clock));
            Clock = 0;
        }

        public void Reset()
        {
            _elapsedTime = TimeSpan.Zero;
            _incompleteTickToExecute = 0;
            _threads.ForEach(p => p.Reset());
        }
    }
}
