using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace ProjectLabSimulator.Displays
{
    internal class St7789Simulated : ISimulatedDisplay
    {
        public St7789Simulated(int width = 240, int height = 240, ColorMode colorMode = ColorMode.Format16bppRgb565)
        {
            this.Width = width;
            this.Height = height;
            this.ColorMode = colorMode;
        }

        public ColorMode ColorMode { get; private set; }
        public Color ForegroundColor => Color.White;
        public Color BackgroundColor => Color.Black;
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}