using System.Collections.Generic;
using Twitch.Models.EventSub.Messages;

namespace Twitch.Utils;

public static class EventSubMessageTypeStringHelper
{
    private static Dictionary<string, EventSubMessageType> Lookup = new()
    {
        ["session_welcome"] = EventSubMessageType.Welcome,
        ["session_keepalive"] = EventSubMessageType.KeepAlive,
        ["notification"] = EventSubMessageType.Notification,
        ["session_reconnection"] = EventSubMessageType.Reconnect,
        ["revocation"] = EventSubMessageType.Revocation,

        // might as well add support for our own thing
        ["unknown"] = EventSubMessageType.Unknown,
    };

    public static EventSubMessageType StringToMessageType(string messageType)
    {
        return Lookup.GetValueOrDefault(messageType, EventSubMessageType.Unknown);
    }

    public static string MessageTypeToString(EventSubMessageType messageType)
    {
        foreach (KeyValuePair<string, EventSubMessageType> kvp in Lookup)
        {
            if (kvp.Value == messageType)
                return kvp.Key;
        }
        return "unknown";
    }
}
