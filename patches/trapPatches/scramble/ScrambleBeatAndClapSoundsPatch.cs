using HarmonyLib;
using RDLevelEditor;

namespace RTwitchTrapIntegration;

public class ScrambleBeatAndClapSoundsPatch : Patch
{
    public static bool ScrambleEnabled = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_AddClassicBeat), nameof(LevelEvent_AddClassicBeat.Decode))]
    [HarmonyPatch(typeof(LevelEvent_AddOneshotBeat), nameof(LevelEvent_AddOneshotBeat.Decode))]
    public static void ClassicBeatsoundPostfix(LevelEvent_Base __instance)
    {
        if (!Config.RunPatches)
            return;
        SoundDataStruct? sound = null;

        if (__instance is LevelEvent_AddClassicBeat classy)
            sound = classy.sound;
        if (__instance is LevelEvent_AddOneshotBeat oneshot)
            sound = oneshot.sound;
            
        if (sound.HasValue && sound.Value.IsExternalClip())
            ScrambleBeatAndClapSoundsTrap.AddCustomBeatsound(sound.Value.filename);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_SetBeatSound), nameof(LevelEvent_SetBeatSound.Decode))]
    public static void SBSBeatsoundPostfix(LevelEvent_SetBeatSound __instance)
    {
        if (!Config.RunPatches)
            return;

        if (__instance.sound.IsExternalClip())
            ScrambleBeatAndClapSoundsTrap.AddCustomBeatsound(__instance.sound.filename);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_MakeRow), nameof(LevelEvent_MakeRow.Decode))]
    public static void MRBeatsoundPostfix(LevelEvent_MakeRow __instance)
    {
        if (!Config.RunPatches)
            return;

        if (__instance.pulseSound.IsExternalClip())
            ScrambleBeatAndClapSoundsTrap.AddCustomBeatsound(__instance.pulseSound.filename);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_SetClapSounds), nameof(LevelEvent_SetClapSounds.Decode))]
    public static void SCSClapsoundPostfix(LevelEvent_SetClapSounds __instance)
    {
        if (!Config.RunPatches)
            return;

        static void DoSound(SoundDataStruct? sound)
        {
            if (sound.HasValue && sound.Value.IsExternalClip())
                ScrambleBeatAndClapSoundsTrap.AddCustomClapsound(sound.Value.filename);
        }

        DoSound(__instance.p1Sound);
        DoSound(__instance.p2Sound);
        DoSound(__instance.cpuSound);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelEvent_SetBeatSound), nameof(LevelEvent_SetBeatSound.Run))]
    public static void BeatsoundPrefix(SoundData ___soundData)
    {
        if (!ScrambleEnabled)
            return;
        ___soundData.filename = ScrambleBeatAndClapSoundsTrap.SwapBeatsound(___soundData.filename);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelEvent_SetClapSounds), nameof(LevelEvent_SetClapSounds.SetPlayerHitSounds))]
    public static void ClapsoundPrefix(SoundData p1Sound, SoundData p2Sound, SoundData cpuSound)
    {
        if (!ScrambleEnabled)
            return;
        
        p1Sound?.filename = ScrambleBeatAndClapSoundsTrap.SwapClapsound(p1Sound.filename);
        p2Sound?.filename = ScrambleBeatAndClapSoundsTrap.SwapClapsound(p2Sound.filename);
        cpuSound?.filename = ScrambleBeatAndClapSoundsTrap.SwapClapsound(cpuSound.filename);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(scnGame), nameof(scnGame.SetPlayerHitSounds))]
    public static void GameClapsoundPrefix(ref string p1Sound, ref string p2Sound, ref string cpuSound)
    {
        if (!ScrambleEnabled)
            return;
        
        p1Sound = ScrambleBeatAndClapSoundsTrap.SwapClapsound(p1Sound);
        p2Sound = ScrambleBeatAndClapSoundsTrap.SwapClapsound(p2Sound);
        cpuSound = ScrambleBeatAndClapSoundsTrap.SwapClapsound(cpuSound);
    }
}