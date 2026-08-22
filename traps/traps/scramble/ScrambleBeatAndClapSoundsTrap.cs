using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RDLevelEditor;

namespace RTwitchTrapIntegration;

[Trap("ScrambleBeatAndClapSounds", "Scrambles (randomises) all the beat/clap sounds in the current level (including custom beat/clap sounds and future set beat/clap sound events).")]
public class ScrambleBeatAndClapSoundsTrap
{
    public static List<string> AvailableBaseBeatsounds = [
        .. RDEditorConstants.BeatSounds
        .Concat([SoundEffect.StickOld, SoundEffect.ClosedHatOld])
        .Except([SoundEffect.None])
        .Select(sfx => sfx.ToString())
    ];
    public static List<string> AvailableCustomBeatsounds = [];

    public static List<string> AvailableBaseClapsounds = [
        .. RDEditorConstants.ClapSoundsP1
        .Concat(RDEditorConstants.ClapSoundsP2)
        .Concat(RDEditorConstants.ClapSoundsCPU)
        .Select(sfx => sfx.ToString())
    ];
    public static List<string> AvailableCustomClapsounds = [];

    public static List<GameSoundType> ClapSoundTypes = [
        GameSoundType.ClapSoundP1Classic, GameSoundType.ClapSoundP2Classic, GameSoundType.ClapSoundCPUClassic,
        GameSoundType.ClapSoundP1Oneshot, GameSoundType.ClapSoundP2Oneshot, GameSoundType.ClapSoundCPUOneshot
    ];
    public static List<int> ClapSoundsIndex => [.. ClapSoundTypes.Cast<int>()];

    public static Dictionary<string, string> BeatsoundLookup;
    public static Dictionary<string, string> ClapsoundLookup;

    public static Dictionary<string, string> PreviousBeatsoundLookup;
    public static Dictionary<string, string> PreviousClapsoundLookup;

    public static void AddCustomBeatsound(string beatsound)
    {
        AddCustomSound(AvailableCustomBeatsounds, beatsound);
    }

    public static void AddCustomClapsound(string clapsound)
    {
        AddCustomSound(AvailableCustomClapsounds, clapsound);
    }

    public static void AddCustomSound(List<string> customSoundsList, string sound)
    {
        if (!sound.EndsWith("*external"))
            sound += "*external";

        if (!customSoundsList.Contains(sound))
            customSoundsList.Add(sound);
    }

    public static string SwapBeatsound(string beatsound)
    {
        if (string.IsNullOrEmpty(beatsound))
            return beatsound;
        return SwapFilename(beatsound, BeatsoundLookup, PreviousBeatsoundLookup);
    }

    public static string SwapClapsound(string clapsound)
    {
        if (string.IsNullOrEmpty(clapsound))
            return clapsound;
        return SwapFilename(clapsound, ClapsoundLookup, PreviousClapsoundLookup);
    }

    public static string SwapFilename(string filename, Dictionary<string, string> lookup, Dictionary<string, string> previousLookup = null)
    {
        bool hasExt = filename.EndsWith("*external");
        bool hasSnd = !hasExt && filename.StartsWith("snd");
        if (hasSnd) 
            filename = filename["snd".Length..];

        if (previousLookup != null)
            filename = previousLookup[filename];

        string newFilename = lookup[filename];
        if (hasSnd && !newFilename.EndsWith("*external"))
            newFilename = "snd" + newFilename;
        // bad unsafe shit because of bad text.Replace in audiolib finding -_-
        if (newFilename == "Kick")
            newFilename = "sndKick";

        return newFilename;
    }

    public static void GameInstantly()
    {        
        ScrambleBeatAndClapSoundsPatch.ScrambleEnabled = true;

        PreviousBeatsoundLookup = null;
        PreviousClapsoundLookup = null;

        if (BeatsoundLookup != null)
        {
            PreviousBeatsoundLookup = [];
            BeatsoundLookup.Do(kvp => PreviousBeatsoundLookup[kvp.Value] = kvp.Key);
        }

        if (ClapsoundLookup != null)
        {
            PreviousClapsoundLookup = [];
            ClapsoundLookup.Do(kvp => PreviousClapsoundLookup[kvp.Value] = kvp.Key);
        }

        List<string> availableBeatsounds = [.. AvailableBaseBeatsounds, .. AvailableCustomBeatsounds];
        List<string> availableClapsounds = [.. AvailableBaseClapsounds, .. AvailableCustomClapsounds];

        BeatsoundLookup = [];
        ClapsoundLookup = [];

        List<string> shuffledBeatsounds = [.. availableBeatsounds];
        List<string> shuffledClapsounds = [.. availableClapsounds];

        shuffledBeatsounds.Shuffle();
        shuffledClapsounds.Shuffle();

        for (int i = 0; i < availableBeatsounds.Count; i++)
            BeatsoundLookup[availableBeatsounds[i]] = shuffledBeatsounds[i]; 

        for (int i = 0; i < availableClapsounds.Count; i++)
            ClapsoundLookup[availableClapsounds[i]] = shuffledClapsounds[i]; 

        scnGame game = scnGame.instance;

        for (int i = 0; i < game.rows.Length; i++)
        {
            Row row = game.rows[i];
            SoundData pulseSound = row.pulseSounds[0];
            pulseSound.filename = SwapBeatsound(pulseSound.filename);
        }

        foreach (int index in ClapSoundsIndex)
        {
            RDGameSoundData clapSound = RDGameSounds.data.sounds[index];
            clapSound.filename = SwapClapsound(clapSound.filename);
            RDGameSounds.data.sounds[index] = clapSound;
        }
    }
}