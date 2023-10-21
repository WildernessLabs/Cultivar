using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace ProjectLabSimulator.Displays
{
    internal class SSd1309Simulated : ISimulatedDisplay
    {
        public SSd1309Simulated()
        {
            this.Width = 128;
            this.Height = 64;
            this.ColorMode = ColorMode.Format1bpp;
        }

        public ColorMode ColorMode { get; private set; }
        public Color ForegroundColor => Color.Yellow;
        public Color BackgroundColor => Color.Black;
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}