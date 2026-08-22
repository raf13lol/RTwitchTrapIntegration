using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(DebugSettings), nameof(DebugSettings.Auto), MethodType.Getter)]
public class ToggleAutoplayPatch : Patch
{
    public static bool Enabled = false;

    public static void Postfix(ref bool __result)
    {
        if (!Enabled)
            return;
        __result = true;
    }
}