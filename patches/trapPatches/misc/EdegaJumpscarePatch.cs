using System.Collections.Generic;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;

namespace RTwitchTrapIntegration;

public class EdegaJumpscarePatch : Patch
{
    public static RDTiledBackground EdegaJumpscare;
    public static List<Texture> Textures = []; 
    public static List<TrapRedemption> Redemptions = []; 
    private static int lastFrame = 0; 

    [HarmonyPostfix]
    [HarmonyPatch(typeof(scnGame), "Awake")]
    public static void AwakePostfix()
    {
        if (!Config.RunPatches || scnEditor.instance != null)
            return;

        Redemptions.Clear();
        scrVfxControl.instance.GetForegroundTextureLayer(-1).Hide();

        GameObject gameObject = Object.Instantiate(scnGame.instance.customBackground);
        gameObject.transform.SetParent(scrGameManager.instance.parentContainer);
        gameObject.layer = 11;
        gameObject.transform.localPosition = new Vector3(scrVfxControl.instance.RDWidth / 2f, scrVfxControl.instance.RDHeight / 2f, 10f);
        EdegaJumpscare = gameObject.GetComponent<RDTiledBackground>();
        EdegaJumpscare.name = nameof(EdegaJumpscarePatch);
        EdegaJumpscare.Hide();

        lastFrame = 0;

        if (Textures.Count <= 0)
        {
            for (int i = 0; i < 5; i++)
                Textures.Add(Resources.Load<Texture2D>($"InternalLevels/EdegaRave/boo_{i}"));
        }    
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RDTiledBackground), "Update")]
    public static void FGUpdatePostfix(RDTiledBackground __instance)
    {
        if (__instance.name != nameof(EdegaJumpscarePatch) || !__instance.isActiveAndEnabled) 
            return;

        int currentFrame = Textures.IndexOf(__instance.material.mainTexture);
        if (currentFrame < lastFrame)
        {
            __instance.Hide();
            lastFrame = 0;
            Redemptions.Do(r => r.Display());
            Redemptions.Clear();
            return;
        }
        lastFrame = currentFrame;
    }
}