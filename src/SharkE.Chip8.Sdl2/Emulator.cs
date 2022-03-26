using SharkE.Chip8.Configuration;
using SharkE.Chip8.Core;
using SharkE.Chip8.Drivers;
using SharkE.Chip8.Sdl2.Drivers;
using SharkE.Chip8.Threading;
using static SDL2.SDL;

namespace SharkE.Chip8.Sdl2
{
    internal class Emulator : IDisposable
    {
        private const double CpuFrequency = 500;
        private const double TimerFrequency = 60;

        private readonly InputDriver _inputDriver = new();
        private readonly VideoDriver _videoDriver = new();
        private readonly SoundDriver _soundDriver = new();

        private bool _isDisposed;
        private readonly ThreadRunner _threadRunner;
        private readonly Arguments _arguments;

        public Emulator(Arguments arguments)
        {
            _arguments = arguments;

            var systemDrivers = new SystemDrivers
            {
                InputDriver = _inputDriver,
                VideoDriver = _videoDriver,
                SoundDriver = _soundDriver
            };

            var cpuFrequency = new Frequency(CpuFrequency);
            var timerFrequency = cpuFrequency.GetSubFrequency(TimerFrequency);

            var delayTimer = new CpuTimer(timerFrequency);
            var soundTimer = new SoundTimer(timerFrequency, systemDrivers);
            var cpu = new Cpu(cpuFrequency, delayTimer, soundTimer, systemDrivers);

            var rom = File.ReadAllBytes(_arguments.Rom);
            cpu.Load(rom);
            Console.WriteLine($"'{_arguments.Rom}' loaded");

            _threadRunner = new ThreadRunner(cpuFrequency, new IThread[] { cpu, delayTimer, soundTimer });
            _threadRunner.Reset();
        }

        ~Emulator()
        {
            Dispose(false);
        }

        public void Run()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
                throw new Exception(SDL_GetError());

            var romName = Path.GetFileName(_arguments.Rom);
            _videoDriver.Initialize(romName);
            ConfigureDrivers(_videoDriver, _arguments);

            var startCounter = SDL_GetPerformanceCounter();
            float elapsedSeconds = .0f;

            var running = true;
            while (running)
            {
                var elapsedTimeSpan = TimeSpan.FromSeconds(elapsedSeconds);

                while (SDL_PollEvent(out var e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL_EventType.SDL_QUIT:
                            running = false;
                            break;

                        case SDL_EventType.SDL_KEYDOWN:
                            if (e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                                running = false;
                            break;
                    }

                    _inputDriver.PollEvent(e);
                    _videoDriver.PollEvent(e);
                }

                if (!running)
                    continue;

                _threadRunner.Tick(elapsedTimeSpan);

                _videoDriver.Draw();

                var endCounter = SDL_GetPerformanceCounter();
                elapsedSeconds = (endCounter - startCounter) / (float)SDL_GetPerformanceFrequency();
                SDL_SetWindowTitle(_videoDriver.Window, $"{romName} - {1.0f / elapsedSeconds} fps");
                startCounter = endCounter;
            }
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

            if (disposing)
            {
                _videoDriver.Dispose();
            }

            SDL_Quit();

            _isDisposed = true;
        }

        private static void ConfigureDrivers(VideoDriver videoDriver, Arguments arguments)
        {
            if (SDL_GetCurrentDisplayMode(0, out var displayMode) < 0)
                throw new Exception(SDL_GetError());

            var width = arguments.FullScreen ? displayMode.w : VideoConfig.DEFAULT_SCREEN_WIDTH;
            var height = arguments.FullScreen ? displayMode.h : VideoConfig.DEFAULT_SCREEN_HEIGHT;

            if (!string.IsNullOrEmpty(arguments.ScreenResolution))
            {
                var screenResolution = arguments.ScreenResolution.Split('x');
                width = int.Parse(screenResolution[0]);
                height = int.Parse(screenResolution[1]);
            }

            videoDriver.ThemeIndex = (int)arguments.ThemeIndex;

            videoDriver.VideoConfig = new VideoConfig
            {
                FullScreen = arguments.FullScreen,
                PreferredBackBufferWidth = width,
                PreferredBackBufferHeight = height
            };
        }
    }
}
