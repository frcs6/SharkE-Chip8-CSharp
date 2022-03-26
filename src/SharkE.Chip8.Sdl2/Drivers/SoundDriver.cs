using SharkE.Chip8.Drivers;

namespace SharkE.Chip8.Sdl2.Drivers
{
    internal class SoundDriver : ISoundDriver
    {
        public void Beep(int frequency, int duration)
        {
            Task.Run(() => Console.Beep(frequency, duration));
        }
    }
}
