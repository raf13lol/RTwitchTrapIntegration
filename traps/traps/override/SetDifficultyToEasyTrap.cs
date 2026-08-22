namespace RTwitchTrapIntegration;

[Trap("SetDifficultyToEasy", "Sets difficulty to be easy mode (overwrites the current difficulty / forced difficulty).")]
public class SetDifficultyToEasyTrap
{
    public static void GameInstantly(Trap t)
    {
        if (OverrideDifficultyPatch.Enabled && OverrideDifficultyPatch.ForceDefibMode == DefibMode.Easy)
        {
            t.Redemption.Cancel();
            return;
        }

        OverrideDifficultyPatch.Enabled = true;
        OverrideDifficultyPatch.ForceDefibMode = DefibMode.Easy;
    }
}