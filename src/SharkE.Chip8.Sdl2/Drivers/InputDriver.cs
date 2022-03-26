using SharkE.Chip8.Core;
using SharkE.Chip8.Drivers;
using static SDL2.SDL;

namespace SharkE.Chip8.Sdl2.Drivers
{
    internal class InputDriver : IInputDriver
    {
        private static Dictionary<byte, SDL_Keycode> KeyMappings { get; set; } = new()
        {
            { EmulatorKeys.Key1, SDL_Keycode.SDLK_1 },
            { EmulatorKeys.Key2, SDL_Keycode.SDLK_2 },
            { EmulatorKeys.Key3, SDL_Keycode.SDLK_3 },
            { EmulatorKeys.KeyC, SDL_Keycode.SDLK_4 },
            { EmulatorKeys.Key4, SDL_Keycode.SDLK_q },
            { EmulatorKeys.Key5, SDL_Keycode.SDLK_w },
            { EmulatorKeys.Key6, SDL_Keycode.SDLK_e },
            { EmulatorKeys.KeyD, SDL_Keycode.SDLK_r },
            { EmulatorKeys.Key7, SDL_Keycode.SDLK_a },
            { EmulatorKeys.Key8, SDL_Keycode.SDLK_s },
            { EmulatorKeys.Key9, SDL_Keycode.SDLK_d },
            { EmulatorKeys.KeyE, SDL_Keycode.SDLK_f },
            { EmulatorKeys.KeyA, SDL_Keycode.SDLK_z },
            { EmulatorKeys.Key0, SDL_Keycode.SDLK_x },
            { EmulatorKeys.KeyB, SDL_Keycode.SDLK_c },
            { EmulatorKeys.KeyF, SDL_Keycode.SDLK_v }
        };

        private readonly InputState _inputState = new();

        public bool IsAnyKeyDown(out byte keyPressed)
        {
            keyPressed = 0;
            foreach (var keyMap in KeyMappings)
            {
                if (_inputState[keyMap.Value])
                {
                    keyPressed = keyMap.Key;
                    return true;
                }
            }

            return false;
        }

        public void PollEvent(SDL_Event e)
        {
            if (KeyMappings.Values.Contains(e.key.keysym.sym))
            {
                if (e.type == SDL_EventType.SDL_KEYUP)
                    _inputState[e.key.keysym.sym] = false;

                if (e.type == SDL_EventType.SDL_KEYDOWN)
                    _inputState[e.key.keysym.sym] = true;
            }
        }

        public bool IsKeyDown(byte key)
            => _inputState[KeyMappings[key]];

        public bool IsKeyUp(byte key)
            => !_inputState[KeyMappings[key]];
    }
}
