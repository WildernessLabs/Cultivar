using Meadow.Cloud;

namespace Cultivar.Commands
{
    public class Fan : IMeadowCommand
    {
        public bool IsOn { get; set; } = false;
    }
}