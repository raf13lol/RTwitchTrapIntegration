using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(RDStartup), nameof(RDStartup.Setup))]
public class TemplatePatch : Patch
{
    public static void Postfix()
    {
    }
}