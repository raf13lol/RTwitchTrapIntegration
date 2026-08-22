namespace RTwitchTrapIntegration;

[Trap("HalfGameSpeed", "Halves the game speed, stacks with DoubleGameSpeed (do note that it does not use the current game speed as a base).")]
public class HalfGameSpeedTrap
{
    public static void GameInstantly()
    {
        OverrideGameSpeedPatch.Enabled = true;
        OverrideGameSpeedPatch.CurrentSpeed /= 2f;
    }
}