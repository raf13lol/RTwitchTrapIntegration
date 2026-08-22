using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RDLevelEditor;

namespace RTwitchTrapIntegration;

public class Traps
{
    public static List<TrapInfo> TrapsInfo = [];

    public static List<Trap> QueuedPreBarTraps = [];
    public static List<Trap> QueuedBarTraps = [];
    public static List<Trap> QueuedInstantTraps = [];

    public static void Init()
    {
        Type[] traps = [..
            Assembly.GetAssembly(typeof(Trap))
            .GetTypes()
            .Where((t) => t.GetCustomAttribute<TrapAttribute>() != null && !t.Name.Contains("Template"))
        ];

        string[] runContexts = Enum.GetNames(typeof(TrapRunType));
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

        foreach (Type trapType in traps)
        {
            TrapAttribute info = trapType.GetCustomAttribute<TrapAttribute>();
            if (info.IsOtherPluginRequired && !OtherPluginUtils.DetectPlugin(info.RequiredPluginGUID))
                continue;

            List<TrapFunction> funcs = [];

            foreach (string runContext in runContexts)
            {
                MethodInfo method = trapType.GetMethod(runContext, bindingFlags);
                if (method == null)
                    continue;

                // blehhh this isn't that good but idc
                TrapRunType runType = Enum.Parse<TrapRunType>(runContext);
                ParameterInfo[] args = method.GetParameters();

                switch (args.Length)
                {
                    case 0:
                        funcs.Add(new(runType, () => method.Invoke(null, [])));
                        break;

                    case 1:
                        if (args[0].ParameterType == typeof(Trap))
                            funcs.Add(new(runType, (Trap t) => method.Invoke(null, [t]), args[0].Name == "requiredState"));
                        else
                            funcs.Add(new(runType, (string userInput) => method.Invoke(null, [userInput])));
                        break;

                    case 2:
                        funcs.Add(new(runType, (string userInput, Trap t) => method.Invoke(null, [userInput, t]), args[1].Name == "requiredState"));
                        break;
                }
            }

            AddTrap(new(info.Name, info.Description, info.AutomaticallyDisplayRedemption, [.. funcs]));
        }
    }

    public static void CheckRedemption(TrapRedemption redemption)
    {
        foreach (TrapInfo trapInfo in TrapsInfo)
        {
            string configName = Config.TrapsName[trapInfo.Name].Value.Trim();
            if (string.IsNullOrEmpty(configName))
                continue;

            if (configName != redemption.RewardName)
                continue;

            Trap trap = new(trapInfo, redemption);

            Patch.Log.LogMessage($"{trapInfo.Name} has been added to queue");
            if (Config.StatusSignShowRedemptions.Value && trapInfo.AutomaticallyDisplayRedemption)
                RedemptionSignPatch.RedemptionsToShow.Add(redemption);

            foreach (TrapFunction func in trapInfo.Functions)
            {
                switch (func.RunType)
                {
                    case TrapRunType.OnPreBar:
                        if (!QueuedPreBarTraps.Contains(trap))
                            QueuedPreBarTraps.Add(trap);
                        break;

                    case TrapRunType.OnBar:
                        if (!QueuedBarTraps.Contains(trap))
                            QueuedBarTraps.Add(trap);
                        break;

                    case TrapRunType.GameInstantly:
                        if (
                        (scnGame.instance == null || scnEditor.instance != null
                        || !(scnGame.instance?.startTheGameCalled ?? false)) 
                        && !QueuedInstantTraps.Contains(trap))
                            QueuedInstantTraps.Add(trap);
                        else
                            trap.Trigger(TrapRunType.GameInstantly);
                        break;

                    case TrapRunType.Instantly:
                        trap.Trigger(TrapRunType.Instantly);
                        break;
                }
            }
            break;
        }
    }

    public static void AddTrap(TrapInfo trapInfo)
    {
        TrapsInfo.Add(trapInfo);
        Config.AddTrapToConfig(trapInfo.Name, trapInfo.Description);
    }
}