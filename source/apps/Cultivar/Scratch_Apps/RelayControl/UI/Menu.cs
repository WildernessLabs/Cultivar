using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace RelayControl.UI
{
    public class RelayControlScreen
    {
        private DisplayLabel[] labels;
        private DisplayBox highlightBox;

        private const int ItemHeight = 30;

        public int SelectedRow { get; set; } = 0;

        public readonly Color UnselectedTextColor = Color.AntiqueWhite;
        public readonly Color SelectedTextColor = Color.Black;
        public readonly Color SelectionColor = Color.Orange;
        public readonly IFont MenuFont = new Font12x20();

        public RelayControlScreen(DisplayScreen screen)
        {
            labels = new DisplayLabel[4];
            var relayCount = 4;

            var x = 2;
            var y = 0;
            var height = ItemHeight;

            // we compose the screen from the back forward, so put the box on first
            highlightBox = new DisplayBox(0, -1, screen.Width, ItemHeight + 2)
            {
                ForeColor = SelectionColor,
                Filled = true,
            };

            screen.Controls.Add(highlightBox);

            for (var i = 0; i < relayCount; i++)
            {
                labels[i] = new DisplayLabel(
                    left: x,
                    top: i * height,
                    width: screen.Width,
                    height: height)
                {
                    Text = $"Relay #{i+1}",
                    Font = MenuFont,
                    BackColor = Color.Transparent,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                // select the first item
                if (i == 0)
                {
                    labels[i].TextColor = SelectedTextColor;
                }
                else
                {
                    labels[i].TextColor = UnselectedTextColor;
                }

                screen.Controls.Add(labels[i]);


                y += height;
            }
        }

        public void Down()
        {
            if (SelectedRow < labels.Length - 1)
            {
                SelectedRow++;

                Resolver.Log.Info($"MENU SELECTED: {labels[SelectedRow].Text}");

                Draw(SelectedRow - 1, SelectedRow);
            }
        }

        public void Up()
        {
            if (SelectedRow > 0)
            {
                SelectedRow--;

                Resolver.Log.Info($"MENU SELECTED: {labels[SelectedRow].Text}");

                Draw(SelectedRow + 1, SelectedRow);
            }
        }

        public void Draw(int oldRow, int newRow)
        {
            labels[oldRow].TextColor = UnselectedTextColor;
            labels[newRow].TextColor = SelectedTextColor;

            highlightBox.Top = labels[newRow].Top - 1;
        }
    }
}