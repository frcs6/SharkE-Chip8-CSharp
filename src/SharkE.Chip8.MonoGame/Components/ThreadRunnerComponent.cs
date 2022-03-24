using Microsoft.Xna.Framework;
using SharkE.Chip8.Threading;

namespace SharkE.Chip8.MonoGame.Components
{
    internal class ThreadRunnerComponent : GameComponent
    {
        private readonly IThreadRunner _threadRunner;

        public ThreadRunnerComponent(Game game, ThreadRunner threadRunner) :
            base(game)
        {
            _threadRunner = threadRunner;
        }

        public override void Update(GameTime gameTime)
        {
            _threadRunner.Tick(gameTime.ElapsedGameTime);
            base.Update(gameTime);
        }
    }
}
