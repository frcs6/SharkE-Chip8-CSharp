namespace SharkE.Chip8.Drivers
{
    public interface IInputDriver
    {
        bool IsAnyKeyDown(out byte keyPressed);
        bool IsKeyDown(byte key);
        bool IsKeyUp(byte key);
    }
}
