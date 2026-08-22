namespace Twitch.Models.EventSub.Messages;

public class EventSubMessagePayload
{
    public EventSubSessionData session { get; set; }
    public EventSubSubscriptionData subscription { get; set; }

    public object data { get; set; }
}