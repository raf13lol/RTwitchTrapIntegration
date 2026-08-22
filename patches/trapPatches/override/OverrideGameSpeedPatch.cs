

using DG.Tweening;
using HarmonyLib;
using UnityEngine;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(scnGame), "Update")]
public class OverrideGameSpeedPatch : Patch
{
    public static bool Enabled = false;
    public static float CurrentSpeed = 1f;

    public static void Postfix(scnGame __instance)
    {
        if (!Enabled || __instance.paused)
            return;
        RDTime.speed = CurrentSpeed;
        Time.timeScale = CurrentSpeed;
        DOTween.timeScale = CurrentSpeed;
        __instance.visualSpeed = CurrentSpeed;
    }
}