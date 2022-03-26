using static SDL2.SDL;

namespace SharkE.Chip8.Sdl2.Drivers
{
    internal class InputState
    {
        private readonly Dictionary<SDL_Keycode, bool> _state = new();

        public bool this[SDL_Keycode index]
        {
            get
            {
                if (!_state.ContainsKey(index))
                    return false;

                return _state[index];
            }
            set
            {
                if (_state.ContainsKey(index) || value)
                    _state[index] = value;
            }
        }
    }
}
