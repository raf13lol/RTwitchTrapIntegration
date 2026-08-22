using HarmonyLib;
using RDModifications;

namespace RTwitchTrapIntegration;

public class RDMFPatch
{
    public static bool Enabled = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(scnGame), "OnMistakeOrHeal")]
    public static void ShowFPostfix(float weight)
    {
        if (!Enabled)
            return;
        FakeRankOnMistake.FPatch.ShowFPostfix(weight);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Rankscreen), "AdvanceGameover")]
    public static void PreventFadeOutRank()
    {
        if (!Enabled)
            return;
        FakeRankOnMistake.FPatch.PreventFadeOutRank();
    }
}