using System.Linq;
using UnityEngine;

namespace RTwitchTrapIntegration;

[Trap("EdegaJumpscare", "Displays the Edega jumpscare from 6-2.", false)]
public class EdegaJumpscareTrap
{
    public static void GameInstantly(Trap t)
    {
        EdegaJumpscarePatch.EdegaJumpscare.Show(
            [.. EdegaJumpscarePatch.Textures.Select(x => (Texture2D)x)],
            ContentMode.Center, FilterMode.Point,
            Color.white,
            RDSortingLayer.Foreground,
            0,
            new(0, 0),
            21
        );
        EdegaJumpscarePatch.Redemptions.Add(t.Redemption);
    }

}