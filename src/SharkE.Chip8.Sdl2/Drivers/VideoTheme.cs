using System.Drawing;

namespace SharkE.Chip8.Sdl2.Drivers
{
    internal class VideoTheme
    {
        public Color ClearColor { get; set; }

        public IList<Color> Palette { get; set; }

        public VideoTheme() :
            this(Color.Black, new List<Color> { Color.Black, Color.White })
        {
        }

        public VideoTheme(Color clearColor, IList<Color> palette)
        {
            ClearColor = clearColor;
            Palette = palette;
        }
    }
}
