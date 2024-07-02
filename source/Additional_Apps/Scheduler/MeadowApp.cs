using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Audio;
using Meadow.Hardware;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowApp
{
    // Change ProjectLabCoreComputeApp to ProjectLabFeatherApp for V1.x boards
    public class MeadowApp : ProjectLabCoreComputeApp
    {
        private Scheduler scheduler;
        private MicroAudio audio;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            Hardware?.RgbLed?.SetColor(Color.Red);

            Hardware?.Speaker?.SetVolume(0.5f);
            audio = new MicroAudio(Hardware.Speaker);

            scheduler = new Scheduler();
            scheduler.EventElapsed += (s, e) =>
            {
                audio.PlaySystemSound(SystemSoundEffect.Success);
                Resolver.Log.Trace($"scheduler event elapsed: {e.EventId}");
            };

            var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += WifiOnNetworkConnected;

            return base.Initialize();
        }

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");
            Hardware?.RgbLed?.SetColor(Color.Yellow);

            await Task.Delay(Timeout.Infinite);
        }

        private void WifiOnNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Trace("network connected");
            Hardware?.RgbLed?.SetColor(Color.Green);

            Task.Delay(2000); // wait for the network time to get set.

            scheduler.AddEventUtc("my event", 23, 28, 00);
            scheduler.AddEventUtc("my event 2", 23, 29, 00);
            scheduler.Start();
        }
    }
}