using System;
using System.Reflection;
using HarmonyLib;
using RDLevelEditor;
using RDModifications;

namespace RTwitchTrapIntegration;

public class RDMBlindfoldedPatch
{
    public static bool Enabled = false;

    public static void CheckToRun(Action func)
    {
        if (!Enabled || Blindfolded.SavedEnabled.Value)
            return;

        Blindfolded.ForceEnable = true;
        func();
        Blindfolded.ForceEnable = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RowEntity), "Setup")]
    [HarmonyPatch(typeof(RowEntity), "Show")]
    [HarmonyPatch(typeof(RowEntity), "DoEntrance")]
    public static void RowPostfix(RowEntity __instance)
    {
        CheckToRun(() => Blindfolded.BlindfoldedVisualsPatch.RowPostfix(__instance));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RowEntity), "Update")]
    public static void UpdatePostfix(RowEntity __instance)
    {
        CheckToRun(() => Blindfolded.BlindfoldedVisualsPatch.UpdatePostfix(__instance));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(scrHoldBar), "visibleHold", MethodType.Getter)]
    public static void HoldBarPostfix(scrHoldBar __instance)
    {
        CheckToRun(() => Blindfolded.BlindfoldedVisualsPatch.HoldBarPostfix(__instance));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RowEntity), "ExpressionPlusFX")]
    public static void FXPrefix(RowEntity __instance)
    {
        CheckToRun(() => Blindfolded.BlindfoldedVisualsPatch.FXPrefix(__instance));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_Sprite), "CreateCharacter")]
    public static void ClassyBeatPostfix(LevelEvent_Sprite __instance)
    {
        CheckToRun(() => Blindfolded.BlindfoldedVisualsPatch.ClassyBeatPostfix(__instance));
    }

    public class RDMBlindfoldedInternalMethodPatch
    {
        public static MethodInfo TargetMethod()
        {
            return AccessUtils.GetFirstMethodContains(typeof(LevelEvent_SetVisible), "<Run>");
        }

        [HarmonyPostfix]
        public static void Postfix(LevelEvent_SetVisible __instance)
        {
            CheckToRun(() => Blindfolded.SetVisibleClassyBeatPatch.Postfix(__instance));
        }
    }
}