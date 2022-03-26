namespace SharkE.Chip8.Configuration
{
    public struct VideoConfig
    {
        public const int DEFAULT_SCREEN_WIDTH = 800;
        public const int DEFAULT_SCREEN_HEIGHT = 600;

        public bool FullScreen { get; set; } = false;
        public int PreferredBackBufferWidth { get; set; } = DEFAULT_SCREEN_WIDTH;
        public int PreferredBackBufferHeight { get; set; } = DEFAULT_SCREEN_HEIGHT;

        public VideoConfig()
        {
        }
    }
}
