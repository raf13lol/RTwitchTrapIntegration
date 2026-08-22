using System.Collections.Generic;
using Twitch.Models.EventSub.Conditions;
using Twitch.Models.Types;

namespace Twitch.Data;

internal class EventSubTable
{
    public static Dictionary<string, EventSubTableTypeHolder> NotificationTypeToSubTypes = new()
    {
        [EventSubEvents.CustomRewardRedemptionAdd.Type] = new(typeof(EventSubRedemptionAddCondition), typeof(TwitchRedemption))
    };
}