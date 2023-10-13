using Meadow.Cloud;

namespace Cultivar.Commands
{
    public class Heater : IMeadowCommand
    {
        public bool IsOn { get; set; } = false;
    }
}