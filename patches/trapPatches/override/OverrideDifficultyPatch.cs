using HarmonyLib;

namespace RTwitchTrapIntegration;

public class OverrideDifficultyPatch : Patch
{
    public static bool Enabled;
    public static DefibMode ForceDefibMode;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(scnGame), "Update")]
    public static void UpdatePostfix()
    {
        if (!Enabled)
            return;

        if (scnGame.p1DefibMode == ForceDefibMode && scnGame.p2DefibMode == ForceDefibMode)
            return;

        scnGame.p1DefibMode = ForceDefibMode;
        scnGame.p2DefibMode = ForceDefibMode;

        for (int i = 0; i < 5; i++)
        {
            scrHandController handController = scnGame.instance.GetHandControllerFromInt(i);
            handController.leftArm.SetDefibMode(ForceDefibMode);
            handController.rightArm.SetDefibMode(ForceDefibMode);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RDArm), nameof(RDArm.SetDefibMode))]
    public static void ArmPrefix(ref DefibMode defibMode)
    {
        if (!Enabled)
            return;
        defibMode = ForceDefibMode;
    }
}