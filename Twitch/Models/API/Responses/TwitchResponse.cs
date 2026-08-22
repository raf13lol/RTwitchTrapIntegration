namespace Twitch.Models.API.Responses;

public class TwitchResponse<T>
{
    public T data { get; set; }
}