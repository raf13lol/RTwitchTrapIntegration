using Twitch.Models.Types;

namespace RTwitchTrapIntegration;

public class TrapRedemption(TwitchRedemption redemption)
{
    public string ID = redemption.id;

    public string RewardID = redemption.reward.id;
    public string RewardName = redemption.reward.title.Trim();
    public int RewardCost = redemption.reward.cost;

    public string UserName = redemption.user_name;
    public string UserInput = redemption.user_input;

    public string Status = redemption.status.ToUpper();

    public TrapInfo TrapInfo = null;
    public bool RewardRequiresUserInput = false;

    public void Display()
    {
        if (Config.StatusSignShowRedemptions.Value)
            RedemptionSignPatch.RedemptionsToShow.Add(this);    
    }

    public void Cancel()
    {
        SetRedemptionStatus("CANCELED");
    }

    public void Fulfil()
    {
        SetRedemptionStatus("FULFILLED");
    }

    public void SetRedemptionStatus(string status)
    {
        // ! MONO RUNTIME IS FUCKED SO THIS DOESN'T WORK 🙂🙂🙂🙂🙂 COME BACK TO THIS SHIT WHEN IT DOES !!!
        // if (!Config.AutomaticallyUpdateRedemptionStatuses.Value)
        //     return;

        // Status = status;
        // TwitchRequestUpdateRedemptionStatus body = new()
        // {
        //     status = status
        // };
        // string bodyText = JsonConvert.SerializeObject(body);

        // Dictionary<string, string> queries = new()
        // {
        //     ["id"] = ID,
        //     ["broadcaster_id"] = Patch.Twitch.BroadcasterID,
        //     ["reward_id"] = RewardID
        // };
        // JsonContent json = new(bodyText);

        // _ = Patch.Twitch.Auth.MakeRequestQuery(
        //     HttpMethod.Patch,
        //     "channel_points/custom_rewards/redemptions",
        //     queries,
        //     json
        // );
    }
}