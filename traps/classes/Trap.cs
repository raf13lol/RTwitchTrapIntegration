using System;
using System.Linq;
using System.Reflection;

namespace RTwitchTrapIntegration;

public class Trap
{
    public TrapInfo Info;
    public TrapRedemption Redemption;

    public Trap(TrapInfo info, TrapRedemption redemption)
    {
        Info = info;

        Redemption = redemption;
        Redemption.TrapInfo = Info;
        Redemption.RewardRequiresUserInput = Info.RequiresUserInput;
    }

    public object State;

    public bool Trigger(TrapRunType runContext)
    {
        Patch.Log.LogMessage($"{Info.Name} has been triggered with context {runContext}");

        bool allRan = true;
        foreach (TrapFunction function in Info.Functions)
        {
            if (function.RunType != runContext)
                continue;

            if (function.RequiresStateToExist && State == null)
            {
                Patch.Log.LogMessage($"{Info.Name} did not trigger on this context {runContext} as required state was missing (this is fine and not an error)");
                allRan = false;
                continue;
            }

            bool ui = function.RequiresUserInput;
            bool state = function.RequiresState;

            try
            {
            // prob the worst code in this project
            if (ui && state)
                ((Action<string, Trap>)function.Function)(Redemption.UserInput, this);
            else if (ui)
                ((Action<string>)function.Function)(Redemption.UserInput);
            else if (state)
                ((Action<Trap>)function.Function)(this);
            else
                ((Action)function.Function)();
            }
            catch (TargetInvocationException e)
            {
                Patch.Log.LogError(e.GetType());
                Patch.Log.LogError(e.Message);
                Patch.Log.LogError(e.StackTrace);
                Patch.Log.LogError(e.InnerException?.GetType());
                Patch.Log.LogError(e.InnerException?.Message);
                Patch.Log.LogError(e.InnerException?.StackTrace);
            }
        }
        return allRan;
    }

    ~Trap()
    {
        if (Redemption.Status != "CANCELED")
            Redemption.Fulfil();    
    }
}