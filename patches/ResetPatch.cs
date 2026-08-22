using DG.Tweening;
using HarmonyLib;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(scnBase), nameof(scnBase.GoToScene))]
public class ResetPatch : Patch
{
    public static void Postfix()
    {
        if (!Config.RunPatches)
            return;
        if (scnBase.lastSceneName != "scnGame")
            return;

        FragileHeartPatch.Multiplier = 1f;
        RandomVFXPresetTrap.ValidVFXPresets.Clear();
        RandomVFXPresetTrap.VFXPresetsAdded.Clear();

        OverrideEasingPatch.ForceEasings = Ease.Unset;
        OverrideGameSpeedPatch.Enabled = false;
        OverrideGameSpeedPatch.CurrentSpeed = 1f;
        OverrideDifficultyPatch.Enabled = false;

        ToggleAutoplayPatch.Enabled = false;

        ScrambleCharactersPatch.ScrambleEnabled = false;
        ScrambleCharactersTrap.AvailableCustomCharacters.Clear();

        ScrambleBeatAndClapSoundsPatch.ScrambleEnabled = false;
        ScrambleBeatAndClapSoundsTrap.BeatsoundLookup = null;
        ScrambleBeatAndClapSoundsTrap.ClapsoundLookup = null;
        ScrambleBeatAndClapSoundsTrap.AvailableCustomBeatsounds.Clear();
        ScrambleBeatAndClapSoundsTrap.AvailableCustomClapsounds.Clear();

        ReadyToGetSetPatch.Enabled = false;

        RDMFPatch.Enabled = false;
        RDMBlindfoldedPatch.Enabled = false;

        if (ForceSamuraiModePatch.Enabled)
        {
            ForceSamuraiModePatch.Enabled = false;
            RDString.samuraiMode = ForceSamuraiModePatch.PreviousEnabled;
        }

        if (ForceNarrationPatch.Enabled)
        {
            ForceNarrationPatch.Enabled = false;
            if (Narration.IsEnabled != ForceNarrationPatch.PreviousEnabled)
                ForceNarrationPatch.ToggleNarrationMethod.Invoke(ForceNarrationPatch.NarrationInstanceField.GetValue(null), []);
        }
    }
}