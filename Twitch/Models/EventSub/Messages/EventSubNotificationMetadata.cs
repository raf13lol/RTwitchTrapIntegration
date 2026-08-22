namespace Twitch.Models.EventSub.Messages;

public class EventSubNotificationMetadata : EventSubMessageMetadata
{
    public string subscription_type { get; set; }
    public string subscription_version { get; set; }
}