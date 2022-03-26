using CommandLine;

namespace SharkE.Chip8.Sdl2
{
    internal class Arguments
    {
        [Option('r', "rom", Required = true, HelpText = "Rom file")]
        public string Rom { get; set; } = "";

        [Option('f', "fullscreen", Required = false, HelpText = "Full screen")]
        public bool FullScreen { get; set; }

        [Option('s', "screenresolution", Required = false, HelpText = "Screen resolution (ex: 800x600)")]
        public string? ScreenResolution { get; set; }

        [Option('t', "theme", Required = false, HelpText = "Theme index")]
        public uint ThemeIndex { get; set; }
    }
}
