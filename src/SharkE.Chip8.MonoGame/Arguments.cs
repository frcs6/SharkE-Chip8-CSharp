using CommandLine;
using SharkE.Chip8.Configuration;

namespace SharkE.Chip8.MonoGame
{
    internal class Arguments
    {
        [Option('r', "rom", Required = true, HelpText = "Rom file")]
        public string Rom { get; set; } = "";

        [Option('w', "window", Required = false, HelpText = "Window mode")]
        public bool Window { get; set; }

        [Option('s', "screenresolution", Required = false, HelpText = "Screen resolution (ex: 800x600)")]
        public string? ScreenResolution { get; set; }

        [Option('m', "rendermode", Required = false, HelpText = "Render mode (Center, PixelPerfect, Fill)")]
        public RenderMode RenderMode { get; set; } = RenderMode.PixelPerfect;

        [Option('t', "theme", Required = false, HelpText = "Theme index")]
        public uint ThemeIndex { get; set; }

        public bool FullScreen => !Window;
    }
}
