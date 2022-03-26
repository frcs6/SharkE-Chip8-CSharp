using SharkE.Chip8.Configuration;
using SharkE.Chip8.Drivers;
using System.Drawing;
using static SDL2.SDL;

namespace SharkE.Chip8.Sdl2.Drivers
{
    internal class VideoDriver : IVideoDriver
    {
        private static List<VideoTheme> VideoThemes { get; set; } = new List<VideoTheme>
        {
           new VideoTheme (Color.Black, new List<Color> { Color.Black, Color.White }),
           new VideoTheme (Color.White, new List<Color> { Color.White, Color.Black }),
           new VideoTheme (Color.Black, new List<Color> { Color.Black, Color.DarkGreen }),
           new VideoTheme (Color.DarkGreen, new List<Color> { Color.DarkGreen, Color.Black }),
           new VideoTheme (Color.Black, new List<Color> { Color.Black, Color.DarkBlue }),
           new VideoTheme (Color.DarkBlue, new List<Color> { Color.DarkBlue, Color.Black }),
        };

        private readonly InputState _inputState = new();

        private bool _isDisposed;
        private IntPtr _renderer;
        private Rectangle _destinationRectangle = Rectangle.Empty;
        private VideoConfig _videoConfig = new();
        private int[,]? _latestBuffer;
        private byte[]? _colorBuffer;
        private IntPtr _surface;
        private IntPtr _texture;
        private SDL_Rect _screenRect;
        private SDL_Rect _textureRect;

        public int ThemeIndex { get; set; }
        public IntPtr Window { get; private set; }

        public VideoConfig VideoConfig
        {
            get => _videoConfig;
            set
            {
                _destinationRectangle = Rectangle.Empty;
                _videoConfig = value;
                if (Window != IntPtr.Zero)
                {
                    SDL_SetWindowSize(Window, _videoConfig.PreferredBackBufferWidth, _videoConfig.PreferredBackBufferHeight);
                    SDL_SetWindowFullscreen(Window, (uint)(value.FullScreen ? SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0));
                    SDL_ShowCursor(value.FullScreen ? 0 : 1);
                }
            }
        }

        ~VideoDriver()
        {
            Dispose(false);
        }

        unsafe public void FillBuffer(int[,] buffer)
        {
            var width = buffer.GetLength(0);
            var height = buffer.GetLength(1);

            if (_colorBuffer == null)
                _colorBuffer = new byte[4 * width * height];

            int index = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var color = VideoThemes[ThemeIndex].Palette[buffer[x, y]];
                    _colorBuffer[index++] = color.R;
                    _colorBuffer[index++] = color.G;
                    _colorBuffer[index++] = color.B;
                    _colorBuffer[index++] = color.A;
                }
            }

            if (_surface == IntPtr.Zero)
            {
                fixed (byte* ptr = _colorBuffer)
                {
                    _surface = SDL_CreateRGBSurfaceFrom((IntPtr)ptr, width, height, 32, width * 4, 0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000);
                    if (_surface == IntPtr.Zero)
                        throw new Exception(SDL_GetError());
                }
            }

            if (_texture != IntPtr.Zero)
            {
                SDL_DestroyTexture(_texture);
                _texture = IntPtr.Zero;
            }

            _texture = SDL_CreateTextureFromSurface(_renderer, _surface);
            if (_texture == IntPtr.Zero)
                throw new Exception(SDL_GetError());

            _latestBuffer = buffer;
        }

        public void Initialize(string title)
        {
            Window = SDL_CreateWindow(
                        title,
                        SDL_WINDOWPOS_UNDEFINED,
                        SDL_WINDOWPOS_UNDEFINED,
                        VideoConfig.PreferredBackBufferWidth,
                        VideoConfig.PreferredBackBufferHeight,
                        SDL_WindowFlags.SDL_WINDOW_SHOWN | (VideoConfig.FullScreen ? SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0));

            if (Window == IntPtr.Zero)
                throw new Exception(SDL_GetError());

            SDL_ShowCursor(VideoConfig.FullScreen ? 0 : 1);

            _renderer = SDL_CreateRenderer(
                            Window,
                            -1,
                            SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (_renderer == IntPtr.Zero)
                throw new Exception(SDL_GetError());
        }

        public void PollEvent(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP)
                _inputState[e.key.keysym.sym] = false;

            if (e.type == SDL_EventType.SDL_KEYDOWN)
            {
                bool saveKey = false;
                if (e.key.keysym.sym == SDL_Keycode.SDLK_F6 && !_inputState[SDL_Keycode.SDLK_F6])
                {
                    ++ThemeIndex;
                    if (ThemeIndex == VideoThemes.Count)
                        ThemeIndex = 0;
                    if (_latestBuffer != null)
                        FillBuffer(_latestBuffer);
                    saveKey = true;
                }

                if (e.key.keysym.sym == SDL_Keycode.SDLK_F10 && !_inputState[SDL_Keycode.SDLK_F10])
                {
                    var videoConfig = VideoConfig;
                    videoConfig.FullScreen = !VideoConfig.FullScreen;
                    VideoConfig = videoConfig;
                    saveKey = true;
                }

                if (saveKey)
                    _inputState[e.key.keysym.sym] = true;

            }
        }

        public void Draw()
        {
            var currentTheme = VideoThemes[ThemeIndex];

            if (SDL_SetRenderDrawColor(_renderer, currentTheme.ClearColor.R, currentTheme.ClearColor.G, currentTheme.ClearColor.B, currentTheme.ClearColor.A) < 0)
                throw new Exception(SDL_GetError());

            if (SDL_RenderClear(_renderer) < 0)
                throw new Exception(SDL_GetError());

            if (_colorBuffer != null)
            {
                ComputeTextureRect();
                if (SDL_RenderCopy(_renderer, _texture, ref _screenRect, ref _textureRect) < 0)
                    throw new Exception(SDL_GetError());
            }

            SDL_RenderPresent(_renderer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (_texture != IntPtr.Zero)
            {
                SDL_DestroyTexture(_texture);
                _texture = IntPtr.Zero;
            }

            if (_surface != IntPtr.Zero)
            {
                SDL_FreeSurface(_texture);
                _surface = IntPtr.Zero;
            }

            if (_renderer != IntPtr.Zero)
            {
                SDL_DestroyRenderer(_renderer);
                _renderer = IntPtr.Zero;
            }

            if (Window != IntPtr.Zero)
            {
                SDL_DestroyWindow(Window);
                Window = IntPtr.Zero;
            }

            _isDisposed = true;
        }

        private void ComputeTextureRect()
        {
            _screenRect = new SDL_Rect
            {
                x = 0,
                y = 0,
                w = VideoConfig.PreferredBackBufferWidth,
                h = VideoConfig.PreferredBackBufferHeight
            };

            if (_latestBuffer == null)
            {
                _textureRect = _screenRect;
                return;
            }

            var width = _latestBuffer.GetLength(0);
            var height = _latestBuffer.GetLength(1);
            var zoom = Math.Min(VideoConfig.PreferredBackBufferWidth / width, VideoConfig.PreferredBackBufferHeight / height);

            _textureRect = new SDL_Rect
            {
                x = (VideoConfig.PreferredBackBufferWidth - zoom * width) / 2,
                y = (VideoConfig.PreferredBackBufferHeight - zoom * height) / 2,
                w = zoom * width,
                h = zoom * height
            };
        }
    }
}
