using System;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json.Linq;
using Twitch.Data;
using Twitch.Models.EventSub.Messages;
using Twitch.Utils;

namespace Twitch;

public class EventSubMessageParser
{
    public static EventSubMessage Parse(Memory<byte> receiveBuffer, ValueWebSocketReceiveResult res)
    {
        byte[] resText = receiveBuffer[..res.Count].ToArray();
        string resString = Encoding.UTF8.GetString(resText);
        return Parse(resString);
    }

    public static EventSubMessage Parse(string messageText)
    {
        JObject message = JObject.Parse(messageText);
        JToken metadataToken = message["metadata"];
        EventSubMessageType messageType = EventSubMessageTypeStringHelper.StringToMessageType((string)metadataToken["message_type"]);

        EventSubMessageMetadata metadata;
        if (messageType == EventSubMessageType.Notification)
            metadata = metadataToken.ToObject<EventSubNotificationMetadata>();
        else
            metadata = metadataToken.ToObject<EventSubMessageMetadata>();

        EventSubMessage eventMessage = new()
        {
            metadata = metadata,
            payload = null
        };

        JToken payloadToken = message["payload"];
        if (payloadToken == null)
            return eventMessage;

        EventSubMessagePayload payload = new();
        eventMessage.payload = payload;

        JToken sessionToken = payloadToken["session"];
        if (sessionToken != null)
            payload.session = sessionToken.ToObject<EventSubSessionData>();

        JToken subscriptionToken = payloadToken["subscription"];
        if (subscriptionToken != null)
            payload.subscription = subscriptionToken.ToObject<EventSubSubscriptionData>();

        if (messageType == EventSubMessageType.Notification)
        {
            EventSubNotificationMetadata notificationMetadata = (EventSubNotificationMetadata)metadata;
            bool exists = EventSubTable.NotificationTypeToSubTypes.TryGetValue(
                notificationMetadata.subscription_type,
                out EventSubTableTypeHolder typeHolder
            );

            if (!exists)
                return eventMessage;

            payload.subscription.condition = subscriptionToken["condition"].ToObject(typeHolder.ConditionType);

            JToken dataToken = payloadToken["event"];
            if (dataToken != null)
                payload.data = dataToken.ToObject(typeHolder.DataType);
        }

        return eventMessage;
    }
}