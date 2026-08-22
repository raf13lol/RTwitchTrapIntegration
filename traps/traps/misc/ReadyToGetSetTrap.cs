namespace RTwitchTrapIntegration;

[Trap("ReadyToGetSet", "Swaps Rea with Get and Dy with Set when the phrases are said.")]
public class ReadyToGetSetTrap
{
    public static void Instantly(Trap t)
    {
        if (ReadyToGetSetPatch.Enabled)
        {
            t.Redemption.Cancel();
            return;
        }
        ReadyToGetSetPatch.Enabled = true;
    }
}