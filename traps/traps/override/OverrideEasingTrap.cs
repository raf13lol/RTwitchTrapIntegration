using System;
using DG.Tweening;

namespace RTwitchTrapIntegration;

[Trap("OverrideEasing", "Forces all easings to be either the easing the user input or a random easing.")]
public class OverrideEasingTrap
{
    public static Ease GetRandomEasing()
    {
        return (Ease)UnityEngine.Random.Range((int)Ease.Linear, (int)Ease.Flash);
    }

    public static void Instantly(string userInput, Trap t)
    {
        string sanitisedUserInput = userInput.ToLower().Replace("ease", "").Replace("flash", "");
        bool shouldBeRandomised = !Enum.TryParse(sanitisedUserInput, true, out Ease res);
        if (shouldBeRandomised)
            res = GetRandomEasing();

        if (OverrideEasingPatch.ForceEasings == res)
        {
            if (!shouldBeRandomised)
            {
                t.Redemption.Cancel();
                return;
            }

            while (OverrideEasingPatch.ForceEasings == res)
                res = GetRandomEasing();
        }
        OverrideEasingPatch.ForceEasings = res;
    }
}