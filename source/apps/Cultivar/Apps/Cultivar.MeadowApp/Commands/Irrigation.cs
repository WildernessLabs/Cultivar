using Meadow.Cloud;

namespace Cultivar.Commands
{
    public class Irrigation : IMeadowCommand
    {
        public bool IsOn { get; set; } = false;
    }
}