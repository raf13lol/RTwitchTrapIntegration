using System.Collections.Generic;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace RTwitchTrapIntegration;

[Trap("SayReadyGetSetGo", "Says a Ready Get Set Go cue with a tick denominator of a random power of 2 (exponent has a range of [-2, 2]).")]
public class SayRDGSGTrap
{
    public static List<OneshotPhraseToSay> PhraseOrder = [
        OneshotPhraseToSay.JustSayRea,
        OneshotPhraseToSay.JustSayDy,
        OneshotPhraseToSay.JustSayGet,
        OneshotPhraseToSay.JustSaySet,
        OneshotPhraseToSay.JustSayGo,
    ];

    public static List<GameVoiceSource> VoiceSources = [
        GameVoiceSource.IanCalm,  
        GameVoiceSource.IanExcited,  
        GameVoiceSource.Nurse,  
        GameVoiceSource.NurseTired,  
        GameVoiceSource.IanSlow,  
        // None is excluded !
        GameVoiceSource.NurseSwing, 
        GameVoiceSource.NurseSwingCalm, 
    ];

    public static void OnPreBar(Trap t)
    {
        SayRDGSGTrapState state;
        if (t.State == null)
        {
            float tick = 1f / Mathf.Pow(2f, Random.Range(-2, 2));
            LevelEvent_SayReadyGetSetGo sayRDGSG = new()
            {
                rooms = null,
                bar = scrConductor.instance.barNumber,
                phraseToSay = OneshotPhraseToSay.JustSayRea,
                voiceSource = VoiceSources[Random.Range(0, VoiceSources.Count)]
            };

            state = new()
            {
                LevelEvent = sayRDGSG,
                Tick = tick,
                DisplayRedemption = false
            };
            t.State = state;

            float barLength = scrConductor.instance.crotchetsPerBar;
            float initBeat = barLength - tick * 4 + 1f;
            while (initBeat < 1f)
                initBeat += barLength;

            sayRDGSG.beat = initBeat;
        }
        else
            state = (SayRDGSGTrapState)t.State;

        int bar = state.LevelEvent.bar;
        float beat = state.LevelEvent.beat;
        OneshotPhraseToSay phrase = state.LevelEvent.phraseToSay;

        int cpb = scrConductor.instance.crotchetsPerBar + 1;
        while (state.LevelEvent.beat < cpb)
        {
            if (RunSayRDGSG(state, true))
                goto ResetVariablesForBar;
        }

        Traps.QueuedPreBarTraps.Add(t);

    ResetVariablesForBar:
        state.LevelEvent.bar = bar;
        state.LevelEvent.beat = beat;
        state.LevelEvent.phraseToSay = phrase;
    }

    public static void OnBar(Trap requiredState)
    {
        SayRDGSGTrapState state = (SayRDGSGTrapState)requiredState.State;
        if (state.DisplayRedemption)
        {
            requiredState.Redemption.Display();
            return;
        }

        int cpb = scrConductor.instance.crotchetsPerBar;
        while (state.LevelEvent.beat < (cpb + 1))
        {
            if (RunSayRDGSG(state, false))
            {
                state.DisplayRedemption = true;
                goto QueueBarTrap;
            }
        }
        state.LevelEvent.beat -= cpb;
        state.LevelEvent.bar++;

    QueueBarTrap:
        Traps.QueuedBarTraps.Add(requiredState);
    }

    public static bool RunSayRDGSG(SayRDGSGTrapState state, bool preBar)
    {
        bool wrapped = false;

        if (preBar)
            state.LevelEvent.RunPrebar();
        else
            state.LevelEvent.Run();

        int phraseIndex = PhraseOrder.IndexOf(state.LevelEvent.phraseToSay);

        phraseIndex++;
        if (phraseIndex >= PhraseOrder.Count)
        {
            phraseIndex = 0;
            wrapped = true;
        }

        state.LevelEvent.phraseToSay = PhraseOrder[phraseIndex];
        state.LevelEvent.beat += state.Tick;

        return wrapped;
    }

    public class SayRDGSGTrapState
    {
        public LevelEvent_SayReadyGetSetGo LevelEvent;
        public float Tick;
        public bool DisplayRedemption;
    }
}