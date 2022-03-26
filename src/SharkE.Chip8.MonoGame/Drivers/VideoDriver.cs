using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharkE.Chip8.Configuration;
using SharkE.Chip8.Drivers;

namespace SharkE.Chip8.MonoGame.Drivers
{
    internal class VideoDriver : DrawableGameComponent, IVideoDriver
    {
        private static List<VideoTheme> VideoThemes { get; set; } = new List<VideoTheme>
        {
           new VideoTheme (Color.Black, new List<uint> { Color.Black.PackedValue, Color.White.PackedValue }),
           new VideoTheme (Color.White, new List<uint> { Color.White.PackedValue, Color.Black.PackedValue }),
           new VideoTheme (Color.Black, new List<uint> { Color.Black.PackedValue, Color.DarkGreen.PackedValue }),
           new VideoTheme (Color.DarkGreen, new List<uint> { Color.DarkGreen.PackedValue, Color.Black.PackedValue }),
           new VideoTheme (Color.Black, new List<uint> { Color.Black.PackedValue, Color.DarkBlue.PackedValue }),
           new VideoTheme (Color.DarkBlue, new List<uint> { Color.DarkBlue.PackedValue, Color.Black.PackedValue }),
        };

        private readonly GraphicsDeviceManager _graphics;
        private Rectangle _destinationRectangle = Rectangle.Empty;
        private SpriteBatch? _spriteBatch;
        private VideoConfig _videoConfig = new();
        private int[,]? _latestBuffer;
        private KeyboardState _latestKeyboardState = new();
        private Texture2D? _textureBuffer;

        public int ThemeIndex { get; set; }

        public VideoConfig VideoConfig
        {
            get => _videoConfig;
            set
            {
                _destinationRectangle = Rectangle.Empty;
                _videoConfig = value;
            }
        }

        public VideoDriver(Game game, GraphicsDeviceManager graphics) :
            base(game)
        {
            _graphics = graphics;
        }

        public void FillBuffer(int[,] buffer)
        {
            if (_textureBuffer == null)
                _textureBuffer = new Texture2D(GraphicsDevice, buffer.GetLength(0), buffer.GetLength(1));

            var width = Math.Min(_textureBuffer.Width, buffer.GetLength(0));
            var height = Math.Min(_textureBuffer.Height, buffer.GetLength(1));

            var colorBuffer = new List<uint>(buffer.Length);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    colorBuffer.Add(VideoThemes[ThemeIndex].Palette[buffer[x, y]]);
                }
            }

            _textureBuffer.SetData(colorBuffer.ToArray());

            _latestBuffer = buffer;
        }

        public override void Initialize()
        {
            if (ThemeIndex < 0 || ThemeIndex >= VideoThemes.Count)
                ThemeIndex = 0;

            ChangeGraphicConfig();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (IsKeyPressed(keyboardState, Keys.F6))
            {
                ++ThemeIndex;
                if (ThemeIndex == VideoThemes.Count)
                    ThemeIndex = 0;
                if (_latestBuffer != null)
                    FillBuffer(_latestBuffer);
            }

            if (IsKeyPressed(keyboardState, Keys.F10))
            {
                var videoConfig = VideoConfig;
                videoConfig.FullScreen = !VideoConfig.FullScreen;
                VideoConfig = videoConfig;
                ChangeGraphicConfig();
            }

            base.Update(gameTime);

            _latestKeyboardState = keyboardState;
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(VideoThemes[ThemeIndex].ClearColor);

            base.Draw(gameTime);

            if (_textureBuffer != null)
            {
                if (_destinationRectangle.IsEmpty)
                    _destinationRectangle = GetDestinationRectangle();

                _spriteBatch!.Begin(samplerState: SamplerState.PointClamp);
                _spriteBatch!.Draw(_textureBuffer, _destinationRectangle, Color.White);
                _spriteBatch.End();
            }
        }

        private bool IsKeyPressed(KeyboardState keyboardState, Keys key)
            => _latestKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);

        private Rectangle GetDestinationRectangle()
        {
            if (_textureBuffer == null)
                return new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            var zoom = Math.Min(GraphicsDevice.Viewport.Width / _textureBuffer.Width, GraphicsDevice.Viewport.Height / _textureBuffer.Height);

            return new Rectangle(
                (GraphicsDevice.Viewport.Width - zoom * _textureBuffer.Width) / 2,
                (GraphicsDevice.Viewport.Height - zoom * _textureBuffer.Height) / 2,
                zoom * _textureBuffer.Width,
                zoom * _textureBuffer.Height);
        }

        private void ChangeGraphicConfig()
        {
            _graphics.PreferredBackBufferWidth = VideoConfig.PreferredBackBufferWidth;
            _graphics.PreferredBackBufferHeight = VideoConfig.PreferredBackBufferHeight;
            _graphics.PreferMultiSampling = true;
            _graphics.IsFullScreen = VideoConfig.FullScreen;
            _graphics.ApplyChanges();

            Game.IsMouseVisible = !VideoConfig.FullScreen;
        }
    }
}
