namespace Twitch.Models.Auth;

// returned from POST https://id.twitch.tv/oauth2/token
public class GrantTokenObject
{
    public int status { get; set; }
    public string message { get; set; } 

    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string[] scope { get; set; }
}