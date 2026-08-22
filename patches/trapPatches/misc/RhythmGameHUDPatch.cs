using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RTwitchTrapIntegration;

public class RhythmGameHUDPatch : Patch
{
    public static GameObject HUDCanvas;
    public static GameObject TextBase;

    public static Dictionary<RGHUDTextType, List<RGHUDText>> AllTexts = [];

    public static List<bool> StoredHitOffsets = [];

    public static int Hits => StoredHitOffsets.Count(o => o);
    public static int Misses => StoredHitOffsets.Count(o => !o);
    public static float Mistakes => game.mistakesManager.mistakes;
    public static float EarlyOffset => game.mistakesManager.earlyOffsetsSum;
    public static float LateOffset => game.mistakesManager.lateOffsetsSum;
    public static float TotalOffset => game.mistakesManager.totalOffsetsSum;
    public static int Combo;
    public static int Score;
    public static double AccuracyNumber;
    public static double Accuracy => Math.Max(1, AccuracyNumber) / Math.Max(1, StoredHitOffsets.Count) * 100d;

    private static scnGame game => scnGame.instance;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(scnGame), "Start")]
    public static void StartPrefix(scnGame __instance)
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        HUDCanvas = __instance.game.statusText.gameObject.transform.parent.gameObject;
        TextBase = __instance.rankscreen.header.gameObject;

        List<RGHUDTextType> types = [.. Enum.GetValues(typeof(RGHUDTextType)).Cast<RGHUDTextType>()];
        if (AllTexts.Count <= 0) 
            types.Do(t => AllTexts[t] = []);

        AllTexts.Do(kvp => kvp.Value.Clear());
        StoredHitOffsets.Clear();

        Combo = 0;
        Score = 0;
        AccuracyNumber = 0d;
    }

    public static void AddText(RGHUDTextType type, string templateString)
    {
        RGHUDText text = new(type, templateString);
        AllTexts[type].Add(text);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(scnGame), nameof(scnGame.AddHitOffset))]
    public static void UpdateMostTextPostfix(scnGame __instance, int rowID, OffsetType offsetType)
    {
        if (!Config.RunPatches || scnEditor.instance != null || __instance.rows[rowID].cpuControlled)
            return;

        bool perfect = offsetType == OffsetType.Perfect;
        StoredHitOffsets.Add(perfect);

        if (!perfect)
            Combo = 0;
        else
            Combo++;

        int hits = Hits;
        AllTexts[RGHUDTextType.Hits].Do(t => t.SetTextString(hits));
        
        int misses = Misses;
        AllTexts[RGHUDTextType.Misses].Do(t => t.SetTextString(misses));

        AllTexts[RGHUDTextType.Combo].Do(t => t.SetTextString(Combo));

        double accNumber = 1d;
        switch (offsetType)
        {
            case OffsetType.SlightlyEarly:
            case OffsetType.SlightlyLate:
                accNumber = 0.5d;
                break;
            case OffsetType.VeryEarly:
            case OffsetType.VeryLate:
                accNumber = 0.1d;
                break;
            case OffsetType.Missed:
                accNumber = 0.0d;
                break;
        }

        AccuracyNumber += accNumber;
        double acc = Accuracy;
        AllTexts[RGHUDTextType.Accuracy].Do(t => t.SetTextString(acc));

        Score += Mathf.RoundToInt((float)accNumber * (100 + Combo));
        AllTexts[RGHUDTextType.Score].Do(t => t.SetTextString(Score));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MistakesManager), nameof(MistakesManager.AddMistake))]
    [HarmonyPatch(typeof(MistakesManager), nameof(MistakesManager.RemoveMistake))]
    [HarmonyPatch(typeof(MistakesManager), nameof(MistakesManager.ClearMistakes))]
    public static void UpdateMistakesText()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        float mistakes = Mistakes;
        AllTexts[RGHUDTextType.Mistakes].Do(t => t.SetTextString(mistakes));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MistakesManager), nameof(MistakesManager.AddAbsoluteMistake))]
    public static void UpdateOffsetText()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        float earlyOffset = EarlyOffset;
        AllTexts[RGHUDTextType.EarlyOffset].Do(t => t.SetTextString(earlyOffset));

        float lateOffset = LateOffset;
        AllTexts[RGHUDTextType.LateOffset].Do(t => t.SetTextString(lateOffset));

        float totalOffset = TotalOffset;
        AllTexts[RGHUDTextType.TotalOffset].Do(t => t.SetTextString(totalOffset));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(scnGame), "Update")]
    public static void UpdateFPSDTText()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        float dt = Time.unscaledDeltaTime;
        AllTexts[RGHUDTextType.DeltaTime].Do(t => t.SetTextString(dt));

        double fps = 1d / dt;
        AllTexts[RGHUDTextType.FPS].Do(t => t.SetTextString(fps));
    }

    public class RGHUDText
    {
        public RGHUDTextType Type;
        public string TemplateString;
        public GameObject Object;
        public Text Text;

        public RGHUDText(RGHUDTextType type, string templateString)
        {
            Type = type;
            TemplateString = templateString;

            Object = UnityEngine.Object.Instantiate(TextBase, HUDCanvas.transform);
            Text = Object.GetComponent<Text>();
            Text.fontSize = 10;
            Text.font = RDString.GetAppropiateFontForString(templateString);
            Object.SetActive(true);

            RectTransform rt = Object.GetComponent<RectTransform>();
            rt.anchoredPosition = new(0, 0);
            rt.sizeDelta = new(352, 198);
            rt.pivot = new(UnityEngine.Random.Range(0.1f, 0.9f), UnityEngine.Random.Range(0.1f, 0.9f));
            rt.rotation = Quaternion.Euler(new(0f, 0f, UnityEngine.Random.Range(-30f, 30f)));

            Update();
        }

        public void Update()
        {
            switch (Type)
            {
                case RGHUDTextType.Hits: SetTextString(Hits); break;
                case RGHUDTextType.Misses: SetTextString(Misses); break;
                case RGHUDTextType.Mistakes: SetTextString(Mistakes); break;
                case RGHUDTextType.EarlyOffset: SetTextString(EarlyOffset); break;
                case RGHUDTextType.LateOffset: SetTextString(LateOffset); break;
                case RGHUDTextType.TotalOffset: SetTextString(TotalOffset); break;
                case RGHUDTextType.Combo: SetTextString(Combo); break;
                case RGHUDTextType.Score: SetTextString(Score); break;
                case RGHUDTextType.Accuracy: SetTextString(Accuracy); break;
                case RGHUDTextType.FPS: SetTextString(1d / Time.unscaledDeltaTime); break;
                case RGHUDTextType.DeltaTime: SetTextString(Time.unscaledDeltaTime); break;
                case RGHUDTextType.Useless: Text.text = TemplateString; break;
            }
        }

        public void SetTextString(object value)
        {
            Text.text = TemplateString.Replace("{value}", value.ToString());
        }
    }

    public enum RGHUDTextType
    {
        Hits,
        Misses,
        Mistakes,
        EarlyOffset,
        LateOffset,
        TotalOffset,
        Combo,
        Score,
        Accuracy,
        FPS,
        DeltaTime,
        Useless
    }
}