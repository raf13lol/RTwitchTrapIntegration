using System.Collections.Generic;
using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(scnGame), nameof(scnGame.OnBeat), [])]
public class RedemptionSignPatch : Patch
{
    public static List<TrapRedemption> RedemptionsToShow = [];

    public static void Postfix()
    {
        if (RedemptionsToShow.Count <= 0)
            return;
        TrapRedemption r = RedemptionsToShow[0];

        string template = r.RewardRequiresUserInput ? Config.StatusSignUserInputText.Value : Config.StatusSignText.Value;
        string text = template
        .Replace("{user}", r.UserName)
        .Replace("{reward_name}", r.RewardName)
        .Replace("{reward_cost}", r.RewardCost.ToString())
        .Replace("{trap_name}", r.TrapInfo.Name);

        if (r.RewardRequiresUserInput)
            text = text.Replace("{user_input}", r.UserInput);

        LEDSign.status = text;
        RedemptionsToShow.RemoveAt(0);
    }
}