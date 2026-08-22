using System.Collections.Generic;
using HarmonyLib;
using RDLevelEditor;

namespace RTwitchTrapIntegration;

[HarmonyPatch(typeof(LevelEvent_SayReadyGetSetGo), "RunInternal")]
public class ReadyToGetSetPatch : Patch
{
    public static bool Enabled = false;

    public static Dictionary<OneshotPhraseToSay, OneshotPhraseToSay> PhrasesToSwap = new()
    {
        [OneshotPhraseToSay.JustSayRea] = OneshotPhraseToSay.JustSayGet,
        [OneshotPhraseToSay.JustSayDy] = OneshotPhraseToSay.JustSaySet,
        [OneshotPhraseToSay.JustSayGet] = OneshotPhraseToSay.JustSayRea,
        [OneshotPhraseToSay.JustSaySet] = OneshotPhraseToSay.JustSayDy,
    };

    public static void Prefix(LevelEvent_SayReadyGetSetGo __instance, out OneshotPhraseToSay __state)
    {
        // should never happen but whatever man
        __state = OneshotPhraseToSay.JustSayGo;
        if (!Enabled)
            return;
        __state = __instance.phraseToSay;
        
        if (PhrasesToSwap.TryGetValue(__instance.phraseToSay, out OneshotPhraseToSay newPhrase))
            __instance.phraseToSay = newPhrase;
    }

    public static void Postfix(LevelEvent_SayReadyGetSetGo __instance, OneshotPhraseToSay __state)
    {
        if (!Enabled)
            return;
        __instance.phraseToSay = __state;
    }
}