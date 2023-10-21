using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace ProjectLabSimulator.Displays
{
    internal class SSd1306Simulated : ISimulatedDisplay
    {
        public SSd1306Simulated(int width = 128, int height = 64)
        {
            this.Width = width;
            this.Height = height;
            this.ColorMode = ColorMode.Format1bpp;
        }

        public ColorMode ColorMode { get; private set; }
        public Color ForegroundColor => Color.Cyan;
        public Color BackgroundColor => Color.Black;
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}