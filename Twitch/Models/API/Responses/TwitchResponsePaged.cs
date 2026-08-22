namespace Twitch.Models.API.Responses;

public class TwitchResponsePaged<T> : TwitchResponse<T>
{
    public TwitchResponsePagination pagination { get; set; }
}