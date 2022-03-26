using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharkE.Chip8.Configuration;
using SharkE.Chip8.Core;
using SharkE.Chip8.Drivers;
using SharkE.Chip8.MonoGame.Components;
using SharkE.Chip8.MonoGame.Drivers;
using SharkE.Chip8.Threading;

namespace SharkE.Chip8.MonoGame
{
    internal class Emulator : Game
    {
        private const double CpuFrequency = 500;
        private const double TimerFrequency = 60;

        public Emulator(Arguments arguments)
        {
            var graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            var inputDriver = new InputDriver(this);
            var videoDriver = new VideoDriver(this, graphics);
            var soundDriver = new SoundDriver(this);
            var systemDrivers = new SystemDrivers
            {
                InputDriver = inputDriver,
                VideoDriver = videoDriver,
                SoundDriver = soundDriver
            };

            ConfigureDrivers(videoDriver, arguments);

            var cpuFrequency = new Frequency(CpuFrequency);
            var timerFrequency = cpuFrequency.GetSubFrequency(TimerFrequency);

            var delayTimer = new CpuTimer(timerFrequency);
            var soundTimer = new SoundTimer(timerFrequency, systemDrivers);
            var cpu = new Cpu(cpuFrequency, delayTimer, soundTimer, systemDrivers);

            var rom = File.ReadAllBytes(arguments.Rom);
            cpu.Load(rom);
            Console.WriteLine($"'{arguments.Rom}' loaded");

            var threadRunner = new ThreadRunner(cpuFrequency, new IThread[] { cpu, delayTimer, soundTimer });
            threadRunner.Reset();

            var frameRateComponent = new FrameRateComponent(this, Path.GetFileName(arguments.Rom));
            var threadRunnerComponent = new ThreadRunnerComponent(this, threadRunner);

            Components.Add(inputDriver);
            Components.Add(videoDriver);
            Components.Add(soundDriver);
            Components.Add(threadRunnerComponent);
            Components.Add(frameRateComponent);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private static void ConfigureDrivers(VideoDriver videoDriver, Arguments arguments)
        {
            var width = arguments.FullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : VideoConfig.DEFAULT_SCREEN_WIDTH;
            var height = arguments.FullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : VideoConfig.DEFAULT_SCREEN_HEIGHT;

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
