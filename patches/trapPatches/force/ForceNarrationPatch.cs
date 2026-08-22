using System.Reflection;
using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(Narration), "Update")]
public class ForceNarrationPatch : Patch
{
    public static bool Enabled = false;
    public static bool PreviousEnabled = false;
    
    public static FieldInfo NarrationInstanceField = AccessTools.Field(typeof(Narration), "instance");
    public static MethodInfo ToggleNarrationMethod = AccessTools.Method(typeof(Narration), "ToggleNarration");

    public static void Postfix(Narration __instance)
    {
        if (!Enabled)
            return;

        if (!Narration.IsEnabled)
            ToggleNarrationMethod.Invoke(__instance, []);
    }
}