using RDModifications;

namespace RTwitchTrapIntegration;

[Trap("FakeRankOnMistake", "Forces F to appear from FakeRankOnMistake from RDModifications.", "com.rhythmdr.randommodifications")]
public class FakeRankOnMistakeTrap
{
    public static void GameInstantly(Trap t)
    {
        if (RDMFPatch.Enabled)
        {
            t.Redemption.Cancel();
            return;
        }
        RDMFPatch.Enabled = true;
        FakeRankOnMistake.FPatch.ForceRank = FakeRankOnMistake.LevelRank.F;
    }
}