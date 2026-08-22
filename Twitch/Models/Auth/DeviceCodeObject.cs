namespace Twitch.Models.Auth;

// returned from POST https://id.twitch.tv/oauth2/device
public class DeviceCodeObject
{
    public string device_code { get; set; }
    public string user_code { get; set; }
    public string verification_uri { get; set; }
    public int expires_in { get; set; }
    public int interval { get; set; }
}