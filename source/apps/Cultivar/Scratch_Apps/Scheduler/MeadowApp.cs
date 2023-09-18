using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Foundation.Audio;
using Meadow.Hardware;

namespace MeadowApp
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IProjectLabHardware projLab;
        private Scheduler scheduler;
        private MicroAudio audio;
        
        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");
            
            projLab = ProjectLab.Create();
            projLab?.RgbLed?.SetColor(Color.Red);
            
            projLab?.Speaker?.SetVolume(0.5f);
            audio = new MicroAudio(projLab.Speaker);
            
            scheduler = new Scheduler();
            scheduler.EventElapsed += (s, e) =>
            {
                audio.PlaySystemSound(SystemSoundEffect.Success);
                Resolver.Log.Trace($"scheduler event elapsed: {e.EventId}");
            };
            
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += WifiOnNetworkConnected;
            
            return base.Initialize();
        }

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");
            projLab?.RgbLed?.SetColor(Color.Yellow);
            
            await Task.Delay(Timeout.Infinite);
        }

        private void WifiOnNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Trace("network connected");
            projLab?.RgbLed?.SetColor(Color.Green);
            
            Task.Delay(2000); // wait for the network time to get set.
            
            scheduler.AddEventUtc("my event", 23, 28, 00);
            scheduler.AddEventUtc("my event 2", 23, 29, 00);
            scheduler.Start();
        }
    }
}