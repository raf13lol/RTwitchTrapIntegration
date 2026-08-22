using System.IO;
using BepInEx;
using BepInEx.Configuration;
#if !BPE5
using BepInEx.Unity.Mono;

#endif
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;

namespace RTwitchTrapIntegration;

[BepInProcess("Rhythm Doctor.exe")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Entry : BaseUnityPlugin
{
    public const bool IsBPE5 =
#if !BPE5 
    false;
#else 
    true;
#endif

#if !BPE5
    public const string DLLName = "com.rhythmdr.rtwitchtrapintegration.dll";
#else
    public const string DLLName = "com.rhythmdr.bpe5rtwitchtrapintegration.dll";
#endif

    public static string UserDataFolder = Path.Combine(Application.dataPath.Replace("Rhythm Doctor_Data", ""), "User");

    public static ConfigEntry<bool> Enabled;

    public static Harmony HarmonyPatcher;
    public static ConfigFile ConfigurationFile;
    public static PluginInfo PluginInfo;

    public void Awake()
    {
        HarmonyPatcher = new("RTwitchTrapInt");
        ConfigurationFile = Config;
        PluginInfo = Info;

        Patch.Log = Logger;

        HarmonyPatcher.PatchAll(typeof(InitPatch));
        HarmonyPatcher.PatchAll(typeof(UpdatePatch));
        HarmonyPatcher.PatchAll(typeof(TrapPatch));
        HarmonyPatcher.PatchAll(typeof(RedemptionSignPatch));
        HarmonyPatcher.PatchAll(typeof(ResetPatch));

        HarmonyPatcher.PatchAll(typeof(EdegaJumpscarePatch));
        HarmonyPatcher.PatchAll(typeof(FragileHeartPatch));
        HarmonyPatcher.PatchAll(typeof(ReadyToGetSetPatch));
        HarmonyPatcher.PatchAll(typeof(RhythmGameHUDPatch));

        HarmonyPatcher.PatchAll(typeof(ForceNarrationPatch));
        HarmonyPatcher.PatchAll(typeof(ForceSamuraiModePatch));
        
        HarmonyPatcher.PatchAll(typeof(OverrideEasingPatch));
        HarmonyPatcher.PatchAll(typeof(OverrideGameSpeedPatch));
        HarmonyPatcher.PatchAll(typeof(OverrideDifficultyPatch));

        HarmonyPatcher.PatchAll(typeof(ToggleAutoplayPatch));

        HarmonyPatcher.PatchAll(typeof(ScrambleCharactersPatch));
        HarmonyPatcher.PatchAll(typeof(ScrambleBeatAndClapSoundsPatch));

        HarmonyPatcher.PatchAll(typeof(RDMFPatch));
        HarmonyPatcher.PatchAll(typeof(RDMBlindfoldedPatch));
        HarmonyPatcher.PatchAll(typeof(RDMBlindfoldedPatch.RDMBlindfoldedInternalMethodPatch));
    }
}

