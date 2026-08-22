namespace Twitch.Models.EventSub.Messages;

public enum EventSubMessageType
{
    Welcome,
    KeepAlive,
    Notification,
    Reconnect,
    Revocation,

    // if we ever get this we've messed up somehow
    Unknown = int.MaxValue
}