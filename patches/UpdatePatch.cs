using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Twitch.Models.Types;
using UnityEngine;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(scnBase), "Update")]
public class UpdatePatch : Patch
{
    public static void Postfix()
    {
        if (!Config.RunPatches)
            return;

        Twitch.Update(Time.unscaledDeltaTime);

        if (Twitch.EventSub.Connected || !Input.GetKeyDown((KeyCode)Config.AuthedKeyCode.Value))
            return;

        Task.Run(async () =>
        {
            if (!await Twitch.TryUseDeviceCodeAndSetup())
                return;
            Log.LogMessage($"Your device code has been accepted; Twitch integration should be working now.");
        });
        Twitch.EventSub.OnNotificationReceived += message =>
        {
            TwitchRedemption redemption = (TwitchRedemption)message.payload.data;
            Traps.CheckRedemption(new(redemption));
        };
    }
}