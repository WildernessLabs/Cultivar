using Meadow.Cloud;

namespace Cultivar.Commands
{
    public class Lights : IMeadowCommand
    {
        public bool IsOn { get; set; } = false;
    }
}