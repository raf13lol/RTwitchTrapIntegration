using System;

namespace RTwitchTrapIntegration;

public class TrapFunction
{
    public TrapFunction(TrapRunType runType, Action function)
    {
        RunType = runType;
        Function = function;
        RequiresUserInput = false;
        RequiresStateToExist = false;
    }

    public TrapFunction(TrapRunType runType, Action<Trap> function, bool requiresStateToExist = false)
    {
        RunType = runType;
        Function = function;
        RequiresState = true;
        RequiresStateToExist = requiresStateToExist;
    }

    public TrapFunction(TrapRunType runType, Action<string> function)
    {
        RunType = runType;
        Function = function;
        RequiresUserInput = true;
        RequiresStateToExist = false;
    }

    public TrapFunction(TrapRunType runType, Action<string, Trap> function, bool requiresStateToExist = false)
    {
        RunType = runType;
        Function = function;
        RequiresUserInput = true;
        RequiresState = true;
        RequiresStateToExist = requiresStateToExist;
    }

    public TrapRunType RunType;
    public bool RequiresUserInput;
    public bool RequiresState;
    public bool RequiresStateToExist;

    public object Function;
}