using System;

namespace RTwitchTrapIntegration;

public class TrapAttribute : Attribute
{
    public TrapAttribute(string name, string desc, bool automaticallyDisplayRedemption)
    {
        Name = name;
        Description = desc;
        RequiredPluginGUID = null;
        AutomaticallyDisplayRedemption = automaticallyDisplayRedemption;
    }

    public TrapAttribute(string name, string desc, string requiredPluginGUID = null, bool automaticallyDisplayRedemption = true)
    {
        Name = name;
        Description = desc;
        RequiredPluginGUID = requiredPluginGUID;
        AutomaticallyDisplayRedemption = automaticallyDisplayRedemption;
    }

    public string Name;
    public string Description;

    public bool IsOtherPluginRequired => !string.IsNullOrEmpty(RequiredPluginGUID);
    public string RequiredPluginGUID;

    public bool AutomaticallyDisplayRedemption;
}