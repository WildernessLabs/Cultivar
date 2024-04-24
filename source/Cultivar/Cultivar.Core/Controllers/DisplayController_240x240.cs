using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp.Controllers;

public class DisplayController_240x240 : IDisplayController
{
    private IPixelDisplay display;
    private DisplayScreen screen;
    private Label statusLabel;

    public DisplayController_240x240(IPixelDisplay display)
    {
        this.display = display;

        CreateLayouts();
    }

    private void CreateLayouts()
    {
        screen = new DisplayScreen(display);
        statusLabel = new Label(0, 0, screen.Width, 20)
        {
            TextColor = Color.White,
            Font = new Font16x24()
        };

        screen.Controls.Add(statusLabel);
    }

    public Task StartConnectingCloudAnimation()
    {
        return Task.CompletedTask;
    }

    public Task StartConnectingWiFiAnimation()
    {
        return Task.CompletedTask;
    }

    public void UpdateCloudStatus(bool IsConnected, bool stopAnimation = false)
    {
    }

    public void UpdateConnectionStatus(bool connected, bool stopAnimation = false)
    {
    }

    public void UpdateHeater(bool on)
    {
    }

    public void UpdateLights(bool on)
    {
    }

    public void UpdateReadings(int logId, double temp, double humidity, double moisture)
    {
    }

    public void UpdateStatus(string status)
    {
        statusLabel.Text = status;
    }

    public void UpdateSync(bool on)
    {
    }

    public void UpdateVents(bool on)
    {
    }

    public void UpdateWater(bool on)
    {
    }
}
