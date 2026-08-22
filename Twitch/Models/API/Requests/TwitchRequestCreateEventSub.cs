using Twitch.Models.Types;

namespace Twitch.Models.API.Requests;

public class TwitchRequestCreateEventSub
{
    public string type { get; set; }
    public string version { get; set; }
    public object condition { get; set; }
    public TwitchTransport transport { get; set; }
}