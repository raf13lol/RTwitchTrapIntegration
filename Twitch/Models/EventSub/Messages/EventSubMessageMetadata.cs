using Newtonsoft.Json;
using Twitch.Utils;

namespace Twitch.Models.EventSub.Messages;

public class EventSubMessageMetadata
{
    public string message_id { get; set; }
    public string message_type { get; set; }

    [JsonIgnore]
    public EventSubMessageType MessageType { get => EventSubMessageTypeStringHelper.StringToMessageType(message_type); }
    
    public string message_timestamp { get; set; }
}