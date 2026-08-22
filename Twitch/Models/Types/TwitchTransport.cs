namespace Twitch.Models.Types;

public class TwitchTransport
{
    public string method { get; set; }

    // WEBHOOK ONLY
    public string callback { get; set; }
    public string secret { get; set; }

    // WEBSOCKET ONLY
    public string session_id { get; set; }

    // CONDUIT ONLY
    public string conduit_id { get; set; }

    // WEBSOCKET RESPONSE ONLY
    public string connected_at { get; set; }
    public string disconnected_at { get; set; }
}