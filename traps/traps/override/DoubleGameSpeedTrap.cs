namespace RTwitchTrapIntegration;

[Trap("DoubleGameSpeed", "Doubles the game speed, stacks with HalfGameSpeed (do note that it does not use the current game speed as a base).")]
public class DoubleGameSpeedTrap
{
    public static void GameInstantly()
    {
        OverrideGameSpeedPatch.Enabled = true;
        OverrideGameSpeedPatch.CurrentSpeed *= 2f;
    }
}