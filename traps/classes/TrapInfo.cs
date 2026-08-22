using System.Linq;

namespace RTwitchTrapIntegration;

public class TrapInfo(string name, string description, bool automaticallyDisplayRedemption, TrapFunction[] functions)
{
    public string Name = name;
    public string Description = description;
    public bool AutomaticallyDisplayRedemption = automaticallyDisplayRedemption;

    public TrapFunction[] Functions = functions;
    public bool RequiresUserInput => Functions.Any(f => f.RequiresUserInput);
}