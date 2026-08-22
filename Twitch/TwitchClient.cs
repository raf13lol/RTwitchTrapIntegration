// INCOMPLETE TWITCH CLIENT INTENDED ONLY FOR RTwitchTrapIntegration 
// ATTEMPTING TO USE FOR OTHER PURPOSES MAY NOT WORK

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RTwitchTrapIntegration;
using Twitch.Models.API.Responses;
using Twitch.Models.API.Types;
using Twitch.Models.Auth;

namespace Twitch;

public class TwitchClient : TwitchObject
{
    public AuthClient Auth;
    public EventSubClient EventSub;
    public string BroadcasterID;

    public TwitchClient(string clientID) : base(clientID)
    {
        Auth = new(clientID);
        EventSub = new(Auth, clientID);
    }

    public async Task<DeviceCodeObject> CreateDeviceAuthorisationRequest()
    {
        return await Auth.CreateDeviceAuthorisationRequest();
    }

    public async Task<bool> TryUseDeviceCodeAndSetup()
    {
        bool deviceCodeRes = await Auth.TryUseDeviceCode();
        if (!deviceCodeRes)
            return false;

        string broadcasterRes = await Auth.MakeRequest(HttpMethod.Get, "users");
        TwitchResponse<TwitchResponseUser[]> usersRes = JsonConvert.DeserializeObject<TwitchResponse<TwitchResponseUser[]>>(broadcasterRes);

        // should be safe to do so
        TwitchResponseUser broadcaster = usersRes.data[0]; 
        BroadcasterID = broadcaster.id;

        await EventSub.Connect();
        await EventSub.Subscribe(BroadcasterID);
        return true;
    }

    public async Task UpdateRedemptionStatus(string rewardID, string redemptionID, string redemptionStatus)
    {
        try
        {
        
        }
        catch (Exception e)
        {
            Patch.Log.LogError(e.GetType().Name);
            Patch.Log.LogError(e.Message);
            Patch.Log.LogError(e.StackTrace);
        }
    }

    public override void Update(float deltaTime)
    {
        if (disposed)
            return;
            
        Auth.Update(deltaTime);
        EventSub.Update(deltaTime);
    }

    public override void Dispose()
    {
        Auth.Dispose();
        EventSub.Dispose();
        base.Dispose();
    }
}