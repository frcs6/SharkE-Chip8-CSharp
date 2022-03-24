using Microsoft.Xna.Framework;

namespace SharkE.Chip8.MonoGame.Components
{
    internal class FrameRateComponent : GameComponent
    {
        private readonly string _title;

        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _frameCounter;
        private int _frameRate;

        public FrameRateComponent(Game game, string title) :
            base(game) => _title = title;

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            ++_frameCounter;

            if (_elapsedTime <= TimeSpan.FromSeconds(1))
                return;

            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            SetTitle(_title, _frameRate);
            _frameCounter = 0;
        }

        public virtual void SetTitle(string title, int framerate)
            => Game.Window.Title = $"{_title} - {_frameRate} fps";
    }
}
