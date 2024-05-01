using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace RelayControl.UI
{
    public static class SplashScreen
    {
        //      protected DisplayScreen Screen { get; set; }

        //      public SplashScreen(DisplayScreen screen)
        //{
        //          Screen = screen;
        //}

        public static void Show(DisplayScreen screen)
        {
            var image = Image.LoadFromResource("img_meadow.bmp");

            screen.Controls.Add(
                new Box(0, 0, screen.Width, screen.Height)
                {
                    ForeColor = Color.White
                },
                new Label(15, 20, 290, 40)
                {
                    Text = "Cultivar",
                    TextColor = WildernessLabsColors.AzureBlue,
                    Font = new Font12x20(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new Picture(90, 74, 140, 90, image)
                {
                    //BackColor = Color.FromHex("#23ABE3"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                });
        }

        //public void Clear()
        //{
        //    Screen.Controls.Clear();
        //}
    }
}

