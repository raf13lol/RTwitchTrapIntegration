namespace RTwitchTrapIntegration;

[Trap("ForceNarration", "Forces narration to be enabled (do not change if narration is enabled yourself when this trap is active).")]
public class ForceNarrationTrap
{
    public static void Instantly(Trap t)
    {
        if (ForceNarrationPatch.Enabled)
        {
            t.Redemption.Cancel();
            return;
        }
        
        ForceNarrationPatch.PreviousEnabled = Narration.IsEnabled;
        ForceNarrationPatch.Enabled = true;
    }
}