using Meadow.Cloud;

namespace MeadowApp.Commands
{
    public class LightControl : IMeadowCommand
    {
        public bool RelayState { get; set; } = false;
    }
}