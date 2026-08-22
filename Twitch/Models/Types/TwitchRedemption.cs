namespace Twitch.Models.Types;

public class TwitchRedemption
{
    public string id { get; set; }
    public string broadcaster_user_id { get; set; }
    public string broadcaster_user_login { get; set; }
    public string broadcaster_user_name { get; set; }

    public string user_id { get; set; }
    public string user_login { get; set; }
    public string user_name { get; set; }
    public string user_input { get; set; }

    public string status { get; set; }
    public string redeemed_at { get; set; }

    public TwitchReward reward { get; set; }
}