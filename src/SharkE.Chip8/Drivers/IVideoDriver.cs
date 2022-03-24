using SharkE.Chip8.Configuration;

namespace SharkE.Chip8.Drivers
{
    public interface IVideoDriver
    {
        VideoConfig VideoConfig { get; set; }

        void FillBuffer(int[,] buffer);
    }
}
