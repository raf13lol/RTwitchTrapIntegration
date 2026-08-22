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
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Traps.CheckRedemption(new(new()
            {
                id = "Let's take a look... 👀👀👀",
                user_name = "raf",
                user_input = "",
                reward = new()
                {
                    title = "rghud"
                },
                status = "unfulfilled"
            }));
        }
        if (Input.GetKeyDown(KeyCode.F4))
            ScrambleCharactersPatch.ScrambleQueued = true;
        if (Input.GetKeyDown(KeyCode.F5) && RandomVFXPresetTrap.VFXPresetsAdded.Count > 0)
        {
            RDThemeFX lastVFX = RandomVFXPresetTrap.VFXPresetsAdded.Pop();
            for (int i = -1; i < 4; i++)
                scnGame.instance.currentLevel.DisableThemeFX(lastVFX, i);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            ScrambleBeatAndClapSoundsTrap.GameInstantly();
        }

        if (Twitch.EventSub.Connected || !Input.GetKeyDown(Config.AuthedKeyCode.Value))
            return;

        Task.Run(async () =>
        {
            if (!await Twitch.TryUseDeviceCodeAndSetup())
                return;
            Log.LogMessage($"Yay it worked your code has been #Approved and your broadcaster id is {Twitch.BroadcasterID}");
        });
        Twitch.EventSub.OnNotificationReceived += message =>
        {
            TwitchRedemption redemption = (TwitchRedemption)message.payload.data;
            Log.LogMessage($"redemption of reward {redemption.reward.title} Claim userinput = {redemption.user_input}");
            Traps.CheckRedemption(new(redemption));
        };
    }
}