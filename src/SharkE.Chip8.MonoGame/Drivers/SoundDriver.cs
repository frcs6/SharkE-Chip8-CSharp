using Microsoft.Xna.Framework;
using SharkE.Chip8.Drivers;

namespace SharkE.Chip8.MonoGame.Drivers
{
    internal class SoundDriver : GameComponent, ISoundDriver
    {
        public SoundDriver(Game game) :
            base(game)
        {
        }

        public void Beep(int frequency, int duration)
        {
            Task.Run(() => Console.Beep(frequency, duration));
        }
    }
}
