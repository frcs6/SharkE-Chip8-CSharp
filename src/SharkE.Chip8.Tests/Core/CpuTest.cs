using Moq;
using FluentAssertions;
using SharkE.Chip8.Core;
using SharkE.Chip8.Drivers;
using SharkE.Chip8.Threading;
using System.IO;
using Xunit;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace SharkE.Chip8.Tests.Core
{
    public class CpuTest
    {
        private static readonly double CpuFrequency = 500;
        private static readonly double TimerFrequency = 60;

        private readonly Mock<IInputDriver> _inputDriver = new();
        private readonly Mock<IVideoDriver> _videoDriver = new();
        private readonly Mock<ISoundDriver> _soundDriver = new();

        private int[,]? _buffer;

        public CpuTest()
        {
            _videoDriver
                .Setup(v => v.FillBuffer(It.IsAny<int[,]>()))
                .Callback<int[,]>(b => _buffer = b);
        }

        [Theory]
        [InlineData("./Roms/c8_test.c8", "./Roms/c8_test.json")]
        [InlineData("./Roms/test_opcode.ch8", "./Roms/test_opcode.json")]
        public void GivenTestRom_WhenTick_ShouldWork(string romPath, string bufferExpectedPath)
        {
            var threadRunner = GetThreadRunner(romPath);
            var expectedBuffer = GetExpectedBuffer(bufferExpectedPath);

            threadRunner.Tick(TimeSpan.FromSeconds(1));

            _buffer.Should().BeEquivalentTo(expectedBuffer);
        }

        private ThreadRunner GetThreadRunner(string romPath)
        {
            var systemDrivers = new SystemDrivers
            {
                InputDriver = _inputDriver.Object,
                VideoDriver = _videoDriver.Object,
                SoundDriver = _soundDriver.Object
            };

            var cpuFrequency = new Frequency(CpuFrequency);
            var timerFrequency = cpuFrequency.GetSubFrequency(TimerFrequency);

            var delayTimer = new CpuTimer(timerFrequency);
            var soundTimer = new SoundTimer(timerFrequency, systemDrivers);
            var cpu = new Cpu(cpuFrequency, delayTimer, soundTimer, systemDrivers);

            var rom = File.ReadAllBytes(romPath);
            cpu.Load(rom);

            var threadRunner = new ThreadRunner(cpuFrequency, new IThread[] { cpu, delayTimer, soundTimer });
            threadRunner.Reset();

            return threadRunner;
        }

        private static int[,] GetExpectedBuffer(string jsonPath)
        {
            var jsonContent = File.ReadAllText(jsonPath);
            var jsonData = JsonSerializer.Deserialize<List<int>>(jsonContent);

            const int X_SIZE = 64;
            const int Y_SIZE = 32;
            var buffer = new int[X_SIZE, Y_SIZE];

            var index = 0;
            for (int y = 0; y < Y_SIZE; ++y)
            {
                for (int x = 0; x < X_SIZE; ++x)
                {
                    buffer[x, y] = jsonData![index++];
                }
            }

            return buffer;
        }
    }
}
