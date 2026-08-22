namespace RTwitchTrapIntegration;

[Trap("FadeOutSong", "The song will fade out and be silent for 2 bars before fading back in.")]
public class FadeOutSongTrap
{
    public static void OnBar(Trap t)
    {
        // first bar
        if (t.State == null)
        {
            t.State = false;
            scnGame.instance.currentLevel.CurrentSongVol(0.025f, scrConductor.instance.crotchet);
        }
        // stall for one bar
        else if (!(bool)t.State) 
            t.State = true;
        // 2 bars after the fade out
        else
        {
            scnGame.instance.currentLevel.CurrentSongVol(1, scrConductor.instance.crotchet);
            return;
        }


        Traps.QueuedBarTraps.Add(t);
    }
}