using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(scnGame), nameof(scnGame.OnMistakeOrHeal))]
public class FragileHeartPatch : Patch
{
    public static float Multiplier = 1f;

    public static void Prefix(ref float weight)
    {
        if (!Config.RunPatches )
            return;
        
        if (weight > 0f)
            weight *= Multiplier;
    }
}