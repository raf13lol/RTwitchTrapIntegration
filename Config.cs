using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace RTwitchTrapIntegration;

public class Config
{
    public static bool Initialised = false;
    public static bool RunPatches => Initialised && Enabled.Value;

    public static ConfigEntry<bool> Enabled;
    public static ConfigEntry<KeyBind> AuthedKeyCode;

    public static ConfigEntry<bool> AutomaticallyUpdateRedemptionStatuses;
    public static ConfigEntry<bool> QueueRedemptionsForNextLevel;
    public static ConfigEntry<bool> RefundRedemptionsOnGameClose;

    public static ConfigEntry<bool> StatusSignShowRedemptions;
    public static ConfigEntry<string> StatusSignText;
    public static ConfigEntry<string> StatusSignUserInputText;

    public static Dictionary<string, ConfigEntry<string>> TrapsName = [];

    private static ConfigFile config => Entry.ConfigurationFile;

    public static void Init()
    {
        Enabled = config.Bind("", "Enabled", false, "If the Twitch integration should be enabled.");
        AuthedKeyCode = config.Bind("", "AuthedKeyCode", KeyBind.F2,
            "Once you have authorised the program through the device code,\n" +
            "the key that should be pressed to indicate to RD to attempt to set up the connection."
        );

        // AutomaticallyUpdateRedemptionStatuses = config.Bind("Settings", "AutomaticallyUpdateRedemptionStatuses", true, 
        // "If the game should try to automatically set redemptions as fulfilled or cancelled.");

        // QueueRedemptionsForNextLevel = config.Bind("Settings", "QueueRedemptionsForNextLevel", true, 
        // "If the game should queue redemptions for the next level when outside of a level.\n" +
        // "If false, the game will refund any redemptions instead of queueing them up.");

        // RefundRedemptionsOnGameClose = config.Bind("Settings", "RefundRedemptionsOnGameClose", true, 
        // "If the game should try to automatically refund redemptions when the game closes.\n" + 
        // "Regardless of this setting, when the game starts, it will check for any unfulfilled redemptions that match a trap.\n" +
        // "The value of QueueRedemptionsForNextLevel determines what the game will do with said redemptions.");

        StatusSignShowRedemptions = config.Bind("Settings", "StatusSignShowRedemptions", true, 
        "For every redemption, whilst in-game, if the status sign should display the rewards redeemed (one redemption displayed per beat).");

        StatusSignText = config.Bind("Settings", "StatusSignText", "{user} redeemed {reward_name}!", 
            "The template text that StatusSignShowRedemptions should show for redemptions that do not use user input.\n" +
            "Strings that will be replaced by the corresponding information are:\n" +
            "{user} = The user who redeemed the reward.\n" +
            "{reward_name} = The name of the reward redeemed.\n" +
            "{reward_cost} = The cost of the reward redeemed.\n" +
            "{trap_name} = The name of the trap trigger."
        );

        StatusSignUserInputText = config.Bind("Settings", "StatusSignUserInputText", "{user} redeemed {reward_name} with input of {user_input}!", 
            "The template text that StatusSignShowRedemptions should show for redemptions that use user input.\n" +
            "Strings that will be replaced by the corresponding information are the same as StatusSignText,\n" + 
            "besides the addition of:\n" +
            "{user_input} = The user input for the reward redemption."
        );

        Initialised = true;
    }

    public static void AddTrapToConfig(string trapName, string trapDescription)
    {
        ConfigEntry<string> trap = config.Bind("Traps", trapName, string.Empty, 
            "The name of the redemption for the trap described below:\n" + 
            $"{trapDescription}\n" + 
            "Leave blank (default) to disable the trap."
        );

        TrapsName[trapName] = trap;        
    }
}