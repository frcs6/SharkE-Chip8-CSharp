using Microsoft.Xna.Framework;

namespace SharkE.Chip8.MonoGame.Drivers
{
    internal class VideoTheme
    {
        public Color ClearColor { get; set; }

        public IList<uint> Palette { get; set; }

        public VideoTheme() :
            this(Color.Black, new List<uint> { Color.Black.PackedValue, Color.White.PackedValue })
        {
        }

        public VideoTheme(Color clearColor, IList<uint> palette)
        {
            ClearColor = clearColor;
            Palette = palette;
        }
    }
}
