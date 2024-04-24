using System.Threading.Tasks;

namespace Cultivar.MeadowApp.Controllers;

public interface IDisplayController
{
    Task StartConnectingCloudAnimation();
    Task StartConnectingWiFiAnimation();
    void UpdateCloudStatus(bool IsConnected, bool stopAnimation = false);
    void UpdateConnectionStatus(bool connected, bool stopAnimation = false);
    void UpdateHeater(bool on);
    void UpdateLights(bool on);
    void UpdateReadings(int logId, double temp, double humidity, double moisture);
    void UpdateStatus(string status);
    void UpdateSync(bool on);
    void UpdateVents(bool on);
    void UpdateWater(bool on);
}