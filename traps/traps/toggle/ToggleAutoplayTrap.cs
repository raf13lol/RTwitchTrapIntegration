namespace RTwitchTrapIntegration;

[Trap("ToggleAutoplay", "Toggles autoplay on and off without any warning.")]
public class ToggleAutoplayTrap
{
    public static void GameInstantly()
    {
        ToggleAutoplayPatch.Enabled = !ToggleAutoplayPatch.Enabled;
    }
}