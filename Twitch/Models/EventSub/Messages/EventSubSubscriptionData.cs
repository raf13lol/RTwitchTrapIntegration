using Twitch.Models.Types;

namespace Twitch.Models.EventSub.Messages;

public class EventSubSubscriptionData
{
    public string id { get; set; }
    public string status { get; set; }
    public string type { get; set; }
    public string version { get; set; }
    public int cost { get; set; }
    public object condition { get; set; }
    public TwitchTransport transport { get; set; }
    public string created_at { get; set; }
}