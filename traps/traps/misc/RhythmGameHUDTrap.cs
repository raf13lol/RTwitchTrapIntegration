using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using static RTwitchTrapIntegration.RhythmGameHUDPatch;

namespace RTwitchTrapIntegration;

[Trap("RhythmGameHUD", "Adds text elements that may provide information on random stats.")]
public class RhythmGameHUDTrap
{
    public static Dictionary<RGHUDTextType, string[]> TemplateTexts = new()
    {
        [RGHUDTextType.Hits] = ["Hits: {value}", "Perfects: {value}", "hids : {value}"],
        [RGHUDTextType.Misses] = ["Misses: {value}", "Combo Breaks: {value}", "aw sh: {value}", "successful tasks failed: {value}"],
        [RGHUDTextType.Mistakes] = ["Mistakes: {value}", "Weighted Misses: {value}", "my basd: {value}", "pipebombs in mailbox: {value}"],
        [RGHUDTextType.EarlyOffset] = ["Early Offset: {value}", "earlys offsets: {value}"],
        [RGHUDTextType.LateOffset] = ["Late Offset: {value}", "lates offsets: {value}"],
        [RGHUDTextType.TotalOffset] = ["Total Offset: {value}", "totals offsets: {value}"],
        [RGHUDTextType.Combo] = ["Combo: {value}", "Streak: {value}"],
        [RGHUDTextType.Score] = ["Score: {value}", "Number that goes up: {value}"],
        [RGHUDTextType.Accuracy] = ["Accuracy: {value}%", "acc: {value}%"],
        [RGHUDTextType.FPS] = ["FPS: {value}", "fps_real: {value}"],
        [RGHUDTextType.DeltaTime] = ["Time.deltaTime: {value}s", "dt: {value}s"],
        [RGHUDTextType.Useless] = [
            "meow", "Lorem ipsum", "\"Value\"", "blahblahblah", 
            "The quick brown fox jumps over the lazy dog", "dQw4w9WgXcQ", "一二三四五六七",
            "ERROR: Could not parse 'weight' of your_mom (value too large)", ":(){ :|:& };:",
            ":3", "had to h", "The Heavy is dead!", "(ﾉ◕ヮ◕​)​ﾉ​*​:​･ﾟ✧",
            "hi", "The Boys Are Back", "achtung!", "What is your green?",
            "You are the greenest person ever!", "LET'S GO GAMBLING!", 
            "your level got peer reviewed!", "your level was non-refereed for the reasons above.",
            "banana detection", "I'm using tilt controls!", "look what you fucking did",
            "fizzd →", "skill issue", "BestFit is only supported for dynamic fonts. Font 'RDLatinFontPoint' is not dynamic.",
            "homslop? decoslop? shatterslop? never heard of them", "Hi i am useless text to fill your screen evil style",
            "The minimum curb weight of the Proterra EcoRide BE35 in lbs: 27,680",
            "The FitnessGram Pacer test is a multistage aerobic capacity test that progressively gets more difficult as it continues. The 20 meter Pacer test will begin in 30 seconds. Line up at the start. The running speed starts slowly, but gets faster each minute after you hear this signal *boop*. A single lap should be completed each time you hear this sound *ding*. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start."
        ],
    };

    public static void GameInstantly()
    {
        RGHUDTextType[] types = [.. Enum.GetValues(typeof(RGHUDTextType)).Cast<RGHUDTextType>()];
        int texts = UnityEngine.Random.Range(4, 7 + 1);

        for (int _ = 0; _ < texts; _++)
        {
            RGHUDTextType type = types[UnityEngine.Random.Range(0, types.Length)];
            string[] templateTexts = TemplateTexts[type];

            int index = UnityEngine.Random.Range(0, templateTexts.Length);
            // the index for fitness gram pacer test so it should be less probable
            if (type == RGHUDTextType.Useless && index == templateTexts.Length - 1)
                index = UnityEngine.Random.Range(0, templateTexts.Length);

            string templateText = templateTexts[index];
            AddText(type, templateText);
        }
    }
}