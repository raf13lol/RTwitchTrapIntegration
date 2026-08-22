using System.Linq;
using System.Reflection;
using DG.Tweening;
using HarmonyLib;

namespace RTwitchTrapIntegration;

public class OverrideEasingPatch : Patch
{
    public static Ease ForceEasings = Ease.Unset;
    public static FieldInfo EaseTypeField = AccessTools.Field(typeof(Tween), "easeType");

    public static MethodInfo TargetMethod()
    {
        return Assembly
        .GetAssembly(typeof(DOTween))
        .GetTypes()
        .Where(t => t.Name == "TweenManager")
        .ToList()[0]
        .GetMethod("Update", AccessTools.all, null, [typeof(Tween), typeof(float), typeof(float), typeof(bool)], []);
    }

    [HarmonyPrefix]
    public static void Prefix(Tween t)
    {
        if (ForceEasings == Ease.Unset || t.debugTargetId == "replacedEasing")
            return; 

        EaseTypeField.SetValue(t, ForceEasings);
        t.debugTargetId = "replacedEasing";
    }
}