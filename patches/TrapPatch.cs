using System.Collections.Generic;
using HarmonyLib;
using RDLevelEditor;

namespace RTwitchTrapIntegration;

public class TrapPatch : Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(scnGame), nameof(scnGame.OnPreBar))]
    public static void PreBarPrefix()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        List<Trap> traps = Traps.QueuedPreBarTraps;
        Traps.QueuedPreBarTraps = [];

        List<Trap> failedTraps = [];
        foreach (Trap trap in traps)
        {
            if (!trap.Trigger(TrapRunType.OnPreBar))
                failedTraps.Add(trap);
        }

        Traps.QueuedPreBarTraps.AddRange(failedTraps);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(scnGame), nameof(scnGame.OnNewBar))]
    public static void BarPrefix()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        List<Trap> traps = Traps.QueuedBarTraps;
        Traps.QueuedBarTraps = [];

        List<Trap> failedTraps = [];
        foreach (Trap trap in traps)
        {
            if (!trap.Trigger(TrapRunType.OnBar))
                failedTraps.Add(trap);
        }

        Traps.QueuedBarTraps.AddRange(failedTraps);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(scnGame), nameof(scnGame.StartTheGame))]
    public static void InstantPrefix()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        foreach (Trap trap in Traps.QueuedInstantTraps)
            trap.Trigger(TrapRunType.GameInstantly);
        Traps.QueuedInstantTraps.Clear();
    }
}