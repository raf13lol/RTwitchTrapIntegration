namespace Twitch.Models.EventSub.Messages;

public class EventSubMessage
{
    public EventSubMessageMetadata metadata { get; set; }
    public EventSubMessagePayload payload { get; set; }
}