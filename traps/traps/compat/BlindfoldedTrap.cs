using System;
using HarmonyLib;
using RDLevelEditor;
using RDModifications;

namespace RTwitchTrapIntegration;

[Trap("Blindfolded", "Forces blindfolded mode on.", "com.rhythmdr.randommodifications")]
public class BlindfoldedTrap
{
    public static void Instantly(Trap t)
    {
        if (RDMBlindfoldedPatch.Enabled)
        {
            t.Redemption.Cancel();
            return;
        }
        RDMBlindfoldedPatch.Enabled = true;

        if (scnGame.instance != null)
        {
            try
            {
                scnGame.instance.rows.Do((r) => r?.ent?.Hide(true, false));
                scnGame.instance.currentLevel.sprites.Do((r) =>
                {
                    CustomSprite customSprite = r.Value;
                    if (customSprite == null || !customSprite.GetComponent<CustomAnimation>().data.name.Contains("classybeat", StringComparison.OrdinalIgnoreCase))
                        return;
                    customSprite.gameObject.SetActive(value: false);
                    customSprite.gameObject.AddComponent<Blindfolded.BlindfoldedMarkedForDeath>();
                });
            }
            catch (Exception e)
            {
                Patch.Log.LogError(e.GetType());
                Patch.Log.LogError(e.Message);
                Patch.Log.LogError(e.StackTrace);
                Patch.Log.LogError("from blindfolded :grin:");
            }
        }
    }
}