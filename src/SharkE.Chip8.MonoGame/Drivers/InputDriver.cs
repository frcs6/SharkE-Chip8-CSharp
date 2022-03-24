using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharkE.Chip8.Core;
using SharkE.Chip8.Drivers;

namespace SharkE.Chip8.MonoGame.Drivers
{
    internal class InputDriver : GameComponent, IInputDriver
    {
        private static Dictionary<byte, int> KeyMappings { get; set; } = new()
        {
            { EmulatorKeys.Key1, (int)Keys.D1 },
            { EmulatorKeys.Key2, (int)Keys.D2 },
            { EmulatorKeys.Key3, (int)Keys.D3 },
            { EmulatorKeys.KeyC, (int)Keys.D4 },
            { EmulatorKeys.Key4, (int)Keys.Q },
            { EmulatorKeys.Key5, (int)Keys.W },
            { EmulatorKeys.Key6, (int)Keys.E },
            { EmulatorKeys.KeyD, (int)Keys.R },
            { EmulatorKeys.Key7, (int)Keys.A },
            { EmulatorKeys.Key8, (int)Keys.S },
            { EmulatorKeys.Key9, (int)Keys.D },
            { EmulatorKeys.KeyE, (int)Keys.F },
            { EmulatorKeys.KeyA, (int)Keys.Z },
            { EmulatorKeys.Key0, (int)Keys.X },
            { EmulatorKeys.KeyB, (int)Keys.C },
            { EmulatorKeys.KeyF, (int)Keys.V }
        };

        private KeyboardState _keyboardState;

        public InputDriver(Game game) :
            base(game)
        {
        }

        public bool IsAnyKeyDown(out byte keyPressed)
        {
            keyPressed = 0;
            foreach (var keyMap in KeyMappings)
            {
                if (_keyboardState.IsKeyDown((Keys)keyMap.Value))
                {
                    keyPressed = keyMap.Key;
                    return true;
                }
            }

            return false;
        }

        public override void Update(GameTime gameTime)
            => _keyboardState = Keyboard.GetState();

        public bool IsKeyDown(byte key)
            => _keyboardState.IsKeyDown((Keys)KeyMappings[key]);

        public bool IsKeyUp(byte key)
            => _keyboardState.IsKeyUp((Keys)KeyMappings[key]);
    }
}
