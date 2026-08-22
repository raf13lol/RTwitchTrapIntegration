using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using RDLevelEditor;

namespace RTwitchTrapIntegration;

public class ScrambleCharactersPatch : Patch
{
    public static bool ScrambleEnabled = false;
    public static bool ScrambleQueued = false;

    private static List<string> customCharactersToPreload = []; 
    private static List<string> availableCC => ScrambleCharactersTrap.AvailableCustomCharacters;

    private const string ocrPrefix = @"|?#<OCR>#?|\/";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RowEntity), nameof(RowEntity.ChangeCharacter))]
    public static void CCPostfix(RowEntity __instance)
    {
        if (!ScrambleEnabled)
            return;
        __instance.freezeshotIceController.Setup(__instance.character.customAnimation.data.freezeTexture);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RowEntity), nameof(RowEntity.ChangeCharacterCustom))]
    public static bool CCCPrefix(RowEntity __instance, string charName, bool showParticles)
    {
        if (!ScrambleEnabled)
            return true;

        // this means we still should run the freezeshotIceController setup
        ScrambleCharactersTrap.CharacterOrCustom character = ScrambleCharactersTrap.GetScrambledCharacter(Character.Custom, charName);
        if (character.Character == Character.Custom)
            return true;

        // this means we shouldn't
        __instance.character.ChangeCharacter(Character.Custom, charName);
        if (showParticles)
            __instance.ShowRowSmokes();
        return false;
    }

    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(scrChar), nameof(scrChar.ChangeCharacter))]
    // public static void ChangeCharacterPrefix(ref Character newChar, ref string customName)
    // {
    //     if (!ScrambleEnabled)
    //         return;

    //     ScrambleCharactersTrap.CharacterOrCustom character = ScrambleCharactersTrap.GetScrambledCharacter(newChar, customName);
    //     newChar = character.Character;
    //     customName = character.CustomCharacterName;
    // }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(scrChar), nameof(scrChar.Setup))]
    [HarmonyPriority(Priority.Last)]
    public static void HandleOCRPrefix(ref Character character, ref string customCharacterName)
    {
        if (!ScrambleEnabled)
            return;

        if (character == Character.Custom && customCharacterName.StartsWith(ocrPrefix))
        {
            string ogCharacter = customCharacterName[ocrPrefix.Length..];
            Dictionary<string, Character> ocrCharNameToChar = new()
            {
                ["olderCole"] = Character.HoodieBoyAlternate,
                ["oldFarmer"] = Character.FarmerAlternate,

                ["oldSamurai"] = Character.Samurai,
                ["oldInsomniac"] = Character.SamuraiBoss,
                ["oldLogan"] = Character.Boy,
                ["oldHailey"] = Character.Girl,
                ["oldCole"] = Character.HoodieBoy,
                ["oldColeBlue"] = Character.HoodieBoyBlue,
                ["olderMiner"] = Character.Miner,
                ["oldIan"] = Character.Ian,

                ["oldBodybuilder"] = Character.Bodybuilder,

                ["oldMiner"] = Character.Miner,
                ["olderPaige"] = Character.Paige,

                ["oldPaige"] = Character.Paige,
                ["oldNicole"] = Character.SmokinBarista,
            };
            character = ocrCharNameToChar[ogCharacter];
            customCharacterName = null;
        }

        ScrambleCharactersTrap.CharacterOrCustom newChar = ScrambleCharactersTrap.GetScrambledCharacter(character, customCharacterName);
        character = newChar.Character;
        if (newChar.Character == Character.Custom)
            customCharacterName = newChar.CustomCharacterName;
        else
            customCharacterName = null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelBase), nameof(LevelBase.baseInit))]
    public static void InitPostfix()
    {
        if (!Config.RunPatches)
            return;

        customCharactersToPreload.Clear();

        if (!ScrambleQueued || scnEditor.instance != null)
            return;

        ScrambleEnabled = true;
        ScrambleQueued = false;

        foreach (string dir in Directory.GetDirectories(LevelValidation.CustomCharactersPath))
        {
            string charName = Path.GetRelativePath(LevelValidation.CustomCharactersPath, dir);
            if (!File.Exists(dir + Path.DirectorySeparatorChar + charName + ".json"))
                continue;

            customCharactersToPreload.Add(charName);
            availableCC.Add(charName);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_MakeRow), nameof(LevelEvent_MakeRow.Decode))]
    public static void RowInitPostfix(LevelEvent_MakeRow __instance)
    {
        if (!Config.RunPatches)
            return;

        if (__instance.character == Character.Custom && !availableCC.Contains(__instance.customCharacterName))
            availableCC.Add(__instance.customCharacterName);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelEvent_Base), nameof(LevelEvent_Base.Decode))]
    public static void ChangeCharInitPostfix(LevelEvent_Base __instance)
    {
        if (!Config.RunPatches || __instance is not LevelEvent_ChangeCharacter changeChar)
            return;

        if (changeChar.character == Character.Custom && !availableCC.Contains(changeChar.customCharacter))
            availableCC.Add(changeChar.customCharacter);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelBase), nameof(LevelBase.LoadCustomAssets))]
    public static IEnumerator PreloadCCFolderPostfix(IEnumerator __result, LevelBase __instance)
    {
        if (ScrambleEnabled)
        {
            ScrambleCharactersTrap.ScrambleCharacters();
            
            // Huh ? unsafe ? might not load in time ? 
            // sounds a lot like pc skill issue tbh
            foreach (string cc in customCharactersToPreload)
                LevelEvent_MakeRow.UpdateCustomCharacter(cc, false, false, true);
        }

        while (__result.MoveNext())
            yield return __result.Current;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RDInk), nameof(RDInk.ParsePortrait))]
    [HarmonyPriority(Priority.Last)]
    public static void ParsePortraitPostfix(string fullName, ref bool isInternal, ref string charName, ref string expression)
    {
        if (!ScrambleEnabled)
            return;

        Character character = isInternal ? Enum.Parse<Character>(charName) : Character.Custom;
        string ccName = isInternal ? null : charName;

        ScrambleCharactersTrap.CharacterOrCustom newCharacter = ScrambleCharactersTrap.GetScrambledCharacter(character, ccName);

        isInternal = newCharacter.Character != Character.Custom;
        charName = isInternal ? newCharacter.Character.ToString() : newCharacter.CustomCharacterName;

        bool noExpression = fullName.IndexOf('_') == -1;
        bool isOCROlderPaige = charName == ocrPrefix + "olderPaige";

        if (!noExpression)
            return;

        if (!isOCROlderPaige && expression == "conversing")
            expression = "neutral";
        else if (isOCROlderPaige && expression == "neutral")
            expression = "conversing";
    }
}