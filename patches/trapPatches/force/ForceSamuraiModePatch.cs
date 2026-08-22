using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(scnBase), "Update")]
public class ForceSamuraiModePatch : Patch
{
    public static bool Enabled = false;
    public static bool PreviousEnabled = false;

    public static void Postfix()
    {
        if (!Enabled)
            return;
        RDString.samuraiMode = true;
    }
}