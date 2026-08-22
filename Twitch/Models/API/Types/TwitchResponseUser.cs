using System;

namespace Twitch.Models.API.Types;

public class TwitchResponseUser
{
    public string id { get; set; }
    public string login { get; set; }
    public string display_name { get; set; }
    public string description { get; set; }

    public string type { get; set; }
    public string broadcaster_type { get; set; }
    
    public string profile_image_url { get; set; }
    public string offline_image_url { get; set; }

    public string created_at { get; set; }

    // Only if user has given us permissions which in the context of this program they won't
    public string email { get; set; }

    [Obsolete]
    public int view_count { get; set; }
}