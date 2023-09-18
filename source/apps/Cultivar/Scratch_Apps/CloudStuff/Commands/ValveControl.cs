using Meadow.Cloud;

namespace MeadowApp.Commands
{
    public class ValveControl : IMeadowCommand
    {
        public bool RelayState { get; set; } = false;
    }
}