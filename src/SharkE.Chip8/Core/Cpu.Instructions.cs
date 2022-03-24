namespace SharkE.Chip8.Core
{
    public partial class Cpu
    {
        /// <summary>
        /// 0x00E0 : CLS
        /// 0x00EE : RET
        /// </summary>
        private void Instructions0()
        {
            var nnn = NNN();

            switch (nnn)
            {
                case 0x0E0:
                    Array.Clear(_display, 0, _display.Length);
                    break;
                case 0x0EE:
                    _programCounter = Pop();
                    break;
            }
        }

        /// <summary>
        /// 0x1nnn : JP addr
        /// </summary>
        private void Instructions1()
            => _programCounter = NNN();

        /// <summary>
        /// 0x2nnn : CALL addr
        /// </summary>
        private void Instructions2()
        {
            Push(_programCounter);
            _programCounter = NNN();
        }

        /// <summary>
        /// 0x3xnn : SE Vx, byte
        /// </summary>
        private void Instructions3()
        {
            var x = X();
            var nn = NN();
            if (_v[x] == nn)
                _programCounter += 2;
        }

        /// <summary>
        /// 0x4xnn : SNE Vx, byte
        /// </summary>
        private void Instructions4()
        {
            var x = X();
            var nn = NN();
            if (_v[x] != nn)
                _programCounter += 2;
        }

        /// <summary>
        /// 0x5xy0 : SE Vx, Vy
        /// </summary>
        private void Instructions5()
        {
            var x = X();
            var y = Y();
            if (_v[x] == _v[y])
                _programCounter += 2;
        }

        /// <summary>
        /// 0x6xkk : LD Vx, byte
        /// </summary>
        private void Instructions6()
        {
            var x = X();
            var nn = NN();
            _v[x] = nn;
        }

        /// <summary>
        /// 0x7xkk : ADD Vx, byte
        /// </summary>
        private void Instructions7()
        {
            var x = X();
            var nn = NN();
            _v[x] += nn;
        }

        /// <summary>
        /// 0x8xy0 : LD Vx, Vy
        /// 0x8xy1 : OR Vx, Vy
        /// 0x8xy2 : AND Vx, Vy
        /// 0x8xy3 : XOR Vx, Vy
        /// 0x8xy4 : ADD Vx, Vy
        /// 0x8xy5 : SUB Vx, Vy
        /// 0x8xy6 : SHR Vx {, Vy}
        /// 0x8xy7 : SUBN Vx, Vy
        /// 0x8xyE : SHL Vx {, Vy}
        /// </summary>
        private void Instructions8()
        {
            var n = N();
            var x = X();
            var y = Y();

            switch (n)
            {
                case 0x0:
                    _v[x] = _v[y];
                    break;
                case 0x1:
                    _v[x] |= _v[y];
                    break;
                case 0x2:
                    _v[x] &= _v[y];
                    break;
                case 0x3:
                    _v[x] ^= _v[y];
                    break;
                case 0x4:
                    _v[0xF] = (byte)(_v[x] + _v[y] > 0xFF ? 1 : 0);
                    _v[x] += _v[y];
                    break;
                case 0x5:
                    _v[0xF] = (byte)(_v[x] > _v[y] ? 1 : 0);
                    _v[x] -= _v[y];
                    break;
                case 0x6:
                    _v[0xF] =
                    _v[0xF] = (byte)(_v[x] & 0x1);
                    _v[x] >>= 1;
                    break;
                case 0x7:
                    _v[0xF] = (byte)(_v[y] > _v[x] ? 1 : 0);
                    _v[x] = (byte)(_v[y] - _v[x]);
                    break;
                case 0xE:
                    _v[0xF] = (byte)(_v[x] >> 7);
                    _v[x] <<= 1;
                    break;
            }
        }

        /// <summary>
        /// 0x9xy0 : SNE Vx, Vy
        /// </summary>
        private void Instructions9()
        {
            var x = X();
            var y = Y();
            if (_v[x] != _v[y])
                _programCounter += 2;
        }

        /// <summary>
        /// 0xAnnn : LD I, addr
        /// </summary>
        private void InstructionsA()
            => _i = NNN();

        /// <summary>
        /// 0xBnnn : JP V0, addr
        /// </summary>
        private void InstructionsB()
            => _programCounter = (ushort)(NNN() + _v[0]);

        /// <summary>
        /// 0xCxkk : RND Vx, byte
        /// </summary>
        private void InstructionsC()
        {
            var x = X();
            var nn = NN();
            _v[x] = (byte)(_random.Next(0, 256) & nn);
        }

        /// <summary>
        /// 0xDxyn : DRW Vx, Vy, nibble
        /// </summary>
        private void InstructionsD()
        {
            var vx = _v[X()];
            var vy = _v[Y()];
            var n = N();

            _v[0xF] = 0;

            for (int row = 0; row < n; ++row)
            {
                var pixels = _memory[_i + row];
                for (int col = 0; col < 8; ++col)
                {
                    if (BitValue(pixels, 7 - col))
                    {
                        var dx = vx + col;
                        var dy = vy + row;

                        if (dx >= _display.GetLength(0) || dy >= _display.GetLength(1))
                            continue;

                        if (_display[dx, dy] == 1)
                            _v[0xF] = 1;

                        _display[dx, dy] ^= 1;
                    }
                }
            }

            _systemDrivers.VideoDriver.FillBuffer(_display);
        }

        /// <summary>
        /// 0xEx9E : SKP Vx
        /// 0xExA1 : SKNP Vx
        /// </summary>
        private void InstructionsE()
        {
            var vx = _v[X()];
            var nn = NN();

            switch (nn)
            {
                case 0x9E:
                    if (_systemDrivers.InputDriver.IsKeyDown(vx))
                        _programCounter += 2;
                    break;
                case 0xA1:
                    if (_systemDrivers.InputDriver.IsKeyUp(vx))
                        _programCounter += 2;
                    break;
            }
        }

        /// <summary>
        /// 0xFx07 : LD Vx, DT
        /// 0xFx0A : LD Vx, K
        /// 0xFx15 : LD DT, Vx
        /// 0xFx18 : LD ST, Vx
        /// 0xFx1E : ADD I, Vx
        /// 0xFx29 : LD F, Vx
        /// 0xFx33 : LD B, Vx
        /// 0xFx55 : LD[I], Vx
        /// 0xFx65 : LD Vx, [I]
        /// </summary>
        private void InstructionsF()
        {
            var nn = NN();
            var x = X();

            switch (nn)
            {
                case 0x07:
                    _v[x] = _delayTimer.Value;
                    break;
                case 0x0A:
                    if (_systemDrivers.InputDriver.IsAnyKeyDown(out var key))
                        _v[x] = key;
                    else
                        _programCounter -= 2;
                    break;
                case 0x15:
                    _delayTimer.Value = _v[x];
                    break;
                case 0x18:
                    _soundTimer.Value = _v[x];
                    break;
                case 0x1E:
                    _i += _v[x];
                    break;
                case 0x29:
                    _i = (ushort)(_v[x] * 5);
                    break;
                case 0x33:
                    _memory[_i + 0] = (byte)((_v[x] / 100) % 10);
                    _memory[_i + 1] = (byte)((_v[x] / 10) % 10);
                    _memory[_i + 2] = (byte)(_v[x] % 10);
                    break;
                case 0x55:
                    for (var i = 0; i <= x; ++i)
                        _memory[_i + i] = _v[i];
                    break;
                case 0x65:
                    for (var i = 0; i <= x; ++i)
                        _v[i] = _memory[_i + i];
                    break;
            }
        }
    }
}
