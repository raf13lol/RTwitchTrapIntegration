namespace RTwitchTrapIntegration;

[Trap("MiawMiaw", "Runs the 'Miaw Miaw!' legacy VFX preset (sets all rows to have nurse counting sounds).")]
public class MiawMiawTrap
{
    public static void GameInstantly(Trap t)
    {
        if (scnGame.instance.nurseCount)
        {
            t.Redemption.Cancel();
            return;
        }
        scnGame.instance.currentLevel.AddThemeFX(RDThemeFX.MiawMiaw);
    }
}