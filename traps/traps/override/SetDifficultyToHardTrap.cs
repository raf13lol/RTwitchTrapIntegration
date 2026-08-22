namespace RTwitchTrapIntegration;

[Trap("SetDifficultyToHard", "Sets difficulty to be hard mode (overwrites the current difficulty / forced difficulty).")]
public class SetDifficultyToHardTrap
{
    public static void GameInstantly(Trap t)
    {
        if (OverrideDifficultyPatch.Enabled && OverrideDifficultyPatch.ForceDefibMode == DefibMode.Hard)
        {
            t.Redemption.Cancel();
            return;
        }
        
        OverrideDifficultyPatch.Enabled = true;
        OverrideDifficultyPatch.ForceDefibMode = DefibMode.Hard;
    }
}