namespace RTwitchTrapIntegration;

[Trap("ForceSamuraiMode", "Forces Samurai. mode to be enabled (do not change if Samurai. mode is enabled yourself when this trap is active).")]
public class ForceSamuraiModeTrap
{
    public static void Instantly(Trap t)
    {
        if (ForceSamuraiModePatch.Enabled)
        {
            t.Redemption.Cancel();
            return;
        }

        ForceSamuraiModePatch.PreviousEnabled = RDString.samuraiMode;
        ForceSamuraiModePatch.Enabled = true;
    }
}