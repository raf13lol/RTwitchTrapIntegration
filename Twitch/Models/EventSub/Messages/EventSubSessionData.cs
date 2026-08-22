namespace Twitch.Models.EventSub.Messages;

public class EventSubSessionData
{
    public string id { get; set; }
    public string status { get; set; }
    public string connected_at { get; set; }
    public int keepalive_timeout_seconds { get; set; }
    public string reconnect_url { get; set; }
    public string recovery_url { get; set; }
}