using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RDLevelEditor;
using Unity.VectorGraphics;

namespace RTwitchTrapIntegration;

[Trap("ScrambleCharacters", "Scrambles (randomises) all the characters in the next level (including custom characters and character change events).")]
public class ScrambleCharactersTrap
{
    public static List<Character> AvailableBaseCharacters = [
        .. Enum
        .GetValues(typeof(Character))
        .Cast<Character>()
        .Except(RDEditorConstants.UnsafeEditorCharacters)
        .Except([Character.None])
    ];

    public static List<string> AvailableCustomCharacters = [];

    public static Dictionary<Character, CharacterOrCustom> CharacterLookup = [];
    public static Dictionary<string, CharacterOrCustom> CustomCharacterLookup = [];

    public static CharacterOrCustom GetScrambledCharacter(Character character, string customCharacterName = null)
    {
        if (character != Character.Custom)
        {
            if (CharacterLookup.TryGetValue(character, out CharacterOrCustom ret))
                return ret;
        }
        else if (!string.IsNullOrEmpty(customCharacterName))
        {
            if (CustomCharacterLookup.TryGetValue(customCharacterName, out CharacterOrCustom ret))
                return ret;
        }

        return new(character)
        {
            CustomCharacterName = customCharacterName  
        };
    }

    public static void ScrambleCharacters()
    {
        CharacterLookup.Clear();
        CustomCharacterLookup.Clear();

        List<CharacterOrCustom> availableCharacters = [
            ..
            AvailableBaseCharacters
            .Select(c => new CharacterOrCustom(c))
            .Concat(
                AvailableCustomCharacters
                .Select(cc => new CharacterOrCustom(cc))
            )
        ];

        List<CharacterOrCustom> shuffledCharacters = [.. availableCharacters];
        // even though this method is from RXRandom, it uses unity rand? so it doesn't mess with the seed ?
        // thaaanks ?
        shuffledCharacters.Shuffle();

        for (int i = 0; i < availableCharacters.Count; i++)
        {
            CharacterOrCustom baseChar = availableCharacters[i];
            CharacterOrCustom shuffledChar = shuffledCharacters[i];

            if (baseChar.Character == Character.Custom)
            {
                CustomCharacterLookup[baseChar.CustomCharacterName] = shuffledChar;
            }
            else
                CharacterLookup[baseChar.Character] = shuffledChar;
        }
    }

    public static void Instantly(Trap t)
    {
        if (ScrambleCharactersPatch.ScrambleQueued)
        {
            t.Redemption.Cancel();
            return;
        }
        ScrambleCharactersPatch.ScrambleQueued = true;
    }

    public class CharacterOrCustom
    {
        public CharacterOrCustom(Character character)
        {
            Character = character;
            CustomCharacterName = null;
        }

        public CharacterOrCustom(string customCharacterName)
        {
            Character = Character.Custom;
            CustomCharacterName = customCharacterName;
        }

        public Character Character;
        public string CustomCharacterName;
    }
}