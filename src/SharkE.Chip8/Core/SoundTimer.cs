using SharkE.Chip8.Drivers;
using SharkE.Chip8.Threading;

namespace SharkE.Chip8.Core
{
    public class SoundTimer : CpuTimer
    {
        private const int BEEP_FREQUENCY = 800;

        private readonly SystemDrivers _systemDrivers;

        private bool _beep;

        public SoundTimer(Frequency targetFrequency, SystemDrivers systemDrivers) :
            base(targetFrequency)
        {
            _systemDrivers = systemDrivers;
        }

        protected override uint Execute()
        {
            if (!_beep && Value > 0)
            {
                Beep();
                _beep = true;
            }

            var tick = base.Execute();

            if (Value == 0)
                _beep = false;

            return tick;
        }

        private void Beep()
        {
            var duration = (int)(1000 * Value / _targetFrequency.Value);
            _systemDrivers.SoundDriver.Beep(BEEP_FREQUENCY, duration);
        }
    }
}
