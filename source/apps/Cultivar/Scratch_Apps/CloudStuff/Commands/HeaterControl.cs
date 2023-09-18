using Meadow.Cloud;

namespace MeadowApp.Commands
{
    public class HeaterControl : IMeadowCommand
    {
        public bool RelayState { get; set; } = false;
    }
}