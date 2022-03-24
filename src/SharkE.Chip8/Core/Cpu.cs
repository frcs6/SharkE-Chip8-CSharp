using SharkE.Chip8.Drivers;
using SharkE.Chip8.Threading;

namespace SharkE.Chip8.Core
{
    public partial class Cpu : BaseThread
    {
        private static readonly byte[] Fonts = {
            0xf0, 0x90, 0x90, 0x90, 0xf0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xf0, 0x10, 0xf0, 0x80, 0xf0, // 2
            0xf0, 0x10, 0xf0, 0x10, 0xf0, // 3
            0x90, 0x90, 0xf0, 0x10, 0x10, // 4
            0xf0, 0x80, 0xf0, 0x10, 0xf0, // 5
            0xf0, 0x80, 0xf0, 0x90, 0xf0, // 6
            0xf0, 0x10, 0x20, 0x40, 0x40, // 7
            0xf0, 0x90, 0xf0, 0x90, 0xf0, // 8
            0xf0, 0x90, 0xf0, 0x10, 0xf0, // 9
            0xf0, 0x90, 0xf0, 0x90, 0x90, // A
            0xe0, 0x90, 0xe0, 0x90, 0xe0, // B
            0xf0, 0x80, 0x80, 0x80, 0xf0, // C
            0xe0, 0x90, 0x90, 0x90, 0xe0, // D
            0xf0, 0x80, 0xf0, 0x80, 0xf0, // E
            0xf0, 0x80, 0xf0, 0x80, 0x80  // F
        };

        private const int KB = 1024;
        private const ushort V_SIZE = 16;
        private const int X_SIZE = 64;
        private const int Y_SIZE = 32;
        private const ushort STACK_SIZE = 16;
        private const int MEMORY_SIZE = 4 * KB;
        private const int PROGRAM_START = 0x200;

        private ushort _i;
        private readonly byte[] _v = new byte[V_SIZE];
        private ushort _programCounter = PROGRAM_START;
        private readonly ushort[] _stack = new ushort[STACK_SIZE];
        private ushort _stackPointer;
        private readonly int[,] _display = new int[X_SIZE, Y_SIZE];
        private readonly byte[] _memory = new byte[MEMORY_SIZE];

        private readonly CpuTimer _delayTimer;
        private readonly CpuTimer _soundTimer;
        private readonly SystemDrivers _systemDrivers;

        private ushort _currentOpcode;
        private readonly Action[] _instructions;
        private Random _random = new();
        private byte[] _rom = Array.Empty<byte>();

        public Cpu(Frequency targetFrequency, CpuTimer delayTimer, CpuTimer soundTimer, SystemDrivers systemDrivers) : 
            base(targetFrequency)
        {
            _delayTimer = delayTimer;
            _soundTimer = soundTimer;
            _systemDrivers = systemDrivers;

            _instructions = new Action[] {
                            Instructions0,
                            Instructions1,
                            Instructions2,
                            Instructions3,
                            Instructions4,
                            Instructions5,
                            Instructions6,
                            Instructions7,
                            Instructions8,
                            Instructions9,
                            InstructionsA,
                            InstructionsB,
                            InstructionsC,
                            InstructionsD,
                            InstructionsE,
                            InstructionsF
            };

            InitializeMemory();
        }

        public void Load(byte[] rom)
        {
            _rom = rom;
            Reset();
        }

        protected override uint Execute()
        {
            _currentOpcode = (ushort)(_memory[_programCounter++] << 8 | _memory[_programCounter++]);
            var instructionsIndex = _currentOpcode >> 12;
            _instructions[instructionsIndex]();
            return 1;
        }

        public override void Reset()
        {
            base.Reset();

            _currentOpcode = 0;
            _random = new Random();

            _i = 0;
            _programCounter = PROGRAM_START;
            _stackPointer = 0;
            Array.Clear(_v, 0, _v.Length);
            Array.Clear(_stack, 0, _stack.Length);
            Array.Clear(_display, 0, _display.Length);

            InitializeMemory();
        }

        private void InitializeMemory()
        {
            Array.Clear(_memory, 0, _memory.Length);
            Array.Copy(Fonts, _memory, Fonts.Length);
            Array.Copy(_rom, 0, _memory, PROGRAM_START, _rom.Length);
        }

        private ushort Pop() => _stack[--_stackPointer];
        private void Push(ushort value) => _stack[_stackPointer++] = value;

        private byte N() => (byte)(_currentOpcode & 0x000F);
        private byte NN() => (byte)(_currentOpcode);
        private ushort NNN() => (ushort)(_currentOpcode & 0x0FFF);

        private byte X() => (byte)((_currentOpcode & 0x0F00) >> 8);
        private byte Y() => (byte)((_currentOpcode & 0x00F0) >> 4);

        private static bool BitValue(byte value, int position) => (value & 1 << position) != 0;
    }
}
