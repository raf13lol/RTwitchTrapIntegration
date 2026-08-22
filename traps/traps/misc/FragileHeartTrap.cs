namespace RTwitchTrapIntegration;

[Trap("FragileHeart", "Multiplies all mistakes by 2x (stacks infinitely).")]
public class FragileHeartTrap
{
    public static void GameInstantly()
    {
        FragileHeartPatch.Multiplier *= 2f;
    }
}