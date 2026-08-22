using HarmonyLib;
using Twitch;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(RDStartup), nameof(RDStartup.Setup))]
public class InitPatch : Patch
{
    public static void Postfix()
    {
        Config.Init();
        Traps.Init();
        if (!Config.Enabled.Value)
        {
            Entry.HarmonyPatcher.UnpatchSelf();
            return;
        }

        TwitchClient twitch = new("6c8dvdxq0uoyvmmmuioh4cw2zx2zmz");
        twitch.Auth.OnDeviceCodeReceived += code =>
        {
            Log.LogMessage($"device code auth needed; type in {code.user_code} / goto {code.verification_uri}");  
        };
        _ = twitch.CreateDeviceAuthorisationRequest();
        Twitch = twitch;
    }
}