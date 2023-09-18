using Meadow.Cloud;

namespace MeadowApp.Commands
{
    public class FanControl : IMeadowCommand
    {
        public bool RelayState { get; set; } = false;
    }
}