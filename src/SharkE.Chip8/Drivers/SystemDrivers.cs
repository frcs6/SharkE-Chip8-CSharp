namespace SharkE.Chip8.Drivers
{
    public struct SystemDrivers
    {
        public IInputDriver InputDriver { get; set; }
        public IVideoDriver VideoDriver { get; set; }
        public ISoundDriver SoundDriver { get; set; }
    }
}
