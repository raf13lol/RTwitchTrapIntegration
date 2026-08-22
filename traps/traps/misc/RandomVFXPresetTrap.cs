using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

namespace RTwitchTrapIntegration;

[Trap("RandomVFXPreset", "Runs a Set VFX Preset with a random vfx preset.")]
public class RandomVFXPresetTrap
{
    public static List<RDThemeFX> ValidVFXPresets = [];
    public static List<RDThemeFX> VFXPresetsAdded = [];

    public static void GameInstantly()
    {
        if (ValidVFXPresets.Count <= 0)
        {
            ValidVFXPresets = [..
                RDEditorConstants.VFXInfos
                .Where(kvp => !kvp.Value.legacy || InspectorPanel_SetVFXPreset.legacyPresetsInLevel.Contains(kvp.Key))
                .Select(kvp => kvp.Key)
                .Except([RDThemeFX.Blackout, RDThemeFX.GlitchObstruction, RDThemeFX.Noise])
            ];
        }

        int index = Random.Range(0, ValidVFXPresets.Count);
        RDThemeFX fx = ValidVFXPresets[index];

        if (fx == RDThemeFX.DisableAll)
            VFXPresetsAdded.Clear();
        else
            VFXPresetsAdded.Add(fx);
        for (int i = -1; i < 4; i++)
        {
            Patch.Log.LogMessage($"{fx} being applied to room {i}");

            RDRoom room = i > -1 ? scnGame.instance.rooms[i] : null;
            if (fx == RDThemeFX.DisableAll)
            {
                if (room != null)
                    room.DisableAllThemeFX();
                else
                {
                    foreach (KeyValuePair<RDThemeFX, RDEditorConstants.VFXInfo> kvp in RDEditorConstants.VFXInfos)
                    {
                        if (kvp.Value.roomsUsage == RoomsUsage.ManyRoomsAndOnTop)
                            scnGame.instance.currentLevel.DisableThemeFX(kvp.Key, -1);
                    }
                }
                continue;
            }

            if (room != null && !room.preloadedVFX.Contains(fx))
            {
                List<RDThemeFX> tempPreloadedVFX = room.preloadedVFX;

                room.preloadedVFX = [fx];
                room.PreloadFX();

                room.preloadedVFX = tempPreloadedVFX;
                room.preloadedVFX.Add(fx);
            }

            float x = Random.Range(0f, 100f);
            float y = Random.Range(0f, 100f);
            if (fx == RDThemeFX.TileN)
            {
                x = Random.Range(2f, 4f);
                y = Random.Range(2f, 4f);
            }
            if (fx == RDThemeFX.CustomScreenScroll)
            {
                x = Random.Range(0.5f, 5f);
                y = Random.Range(0.5f, 5f);
            }

            float intensity = Random.Range(0.75f, 1.25f);
            if (fx == RDThemeFX.Bloom)
                intensity /= 25f;

            float extra = Random.Range(0.75f, 1.25f);
            if (fx == RDThemeFX.Bloom)
                extra = Random.Range(0.1f, 0.75f);

            scnGame.instance.currentLevel.AddThemeFX(fx, i, x, y, intensity, extra, extra, 1, Ease.Linear, Color.white);
        }
    }
}