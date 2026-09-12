using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RTwitchTrapIntegration;
using Twitch.Data;
using Twitch.Models.API.Requests;
using Twitch.Models.EventSub.Conditions;
using Twitch.Models.EventSub.Messages;
using Twitch.Utils;

namespace Twitch;

public class EventSubClient : TwitchObject
{
    public EventSubSocket MainSocket;
    public EventSubSocket BackSocket;

    public bool Connected = false;
    private float timeSinceLastMessage;
    private float keepAliveTime;

    public List<TwitchRequestCreateEventSub> Subscriptions;
    public AuthClient Auth;

    public event EventSubSocket.MessageReceivedCallback OnNotificationReceived;

    public EventSubClient(AuthClient auth, string clientID) : base(clientID)
    {
        Auth = auth;

        MainSocket = new();
        BackSocket = null;

        Connected = false;
        timeSinceLastMessage = 0f;
        keepAliveTime = 10f;

        Subscriptions = [];
    }

    public override void Update(float deltaTime)
    {
        if (!Connected || disposed)
            return;

        if (timeSinceLastMessage > keepAliveTime)
        {
            Patch.Log.LogMessage("Keepalive timeout");
            _ = TimeoutReconnect();
        }
        timeSinceLastMessage += deltaTime;
    }

    public async Task Connect(string reconnectURL = null)
    {
        MainSocket.OnMessageReceived += HandleMessage;
        await MainSocket.Connect(reconnectURL);
    }

    public async Task TimeoutReconnect()
    {
        Connected = false;

        MainSocket.Dispose();
        MainSocket = new();

        await Connect();
        foreach (TwitchRequestCreateEventSub sub in Subscriptions)
            await InternalSubscribe(sub);
    }

    private void HandleMessage(EventSubMessage message)
    {
        timeSinceLastMessage = 0f;
        switch (message.metadata.MessageType)
        {
            case EventSubMessageType.Welcome:
                Connected = true;
                keepAliveTime = message.payload.session.keepalive_timeout_seconds + 5f;

                BackSocket?.Dispose();
                BackSocket = null;
                break;

            case EventSubMessageType.Notification:
                OnNotificationReceived?.Invoke(message);
                break;

            case EventSubMessageType.Reconnect:
                BackSocket = MainSocket;
                MainSocket = new();
                // ehhhh no worries honestly
                _ = Connect(message.payload.session.reconnect_url);
                break;

            case EventSubMessageType.Revocation:
                // ! if we ever get this we are fucked
                Dispose();
                break;

            case EventSubMessageType.KeepAlive:
            case EventSubMessageType.Unknown:
                // do nothing
                break;
        }
    }

    public async Task Subscribe(string broadcasterID)
    {
        TwitchRequestCreateEventSub requestData = new()
        {
            type = EventSubEvents.CustomRewardRedemptionAdd.Type,
            version = EventSubEvents.CustomRewardRedemptionAdd.Version,
            condition = new EventSubRedemptionAddCondition()
            {
                broadcaster_user_id = broadcasterID
            }
        };
        Subscriptions.Add(requestData);

        await InternalSubscribe(requestData);
    }

    private async Task InternalSubscribe(TwitchRequestCreateEventSub data)
    {
        // just in case
        data.transport = new()
        {
            method = "websocket",
            session_id = MainSocket.WebSocketID
        };

        string json = JsonConvert.SerializeObject(data);
        await Auth.MakeRequest(HttpMethod.Post, "eventsub/subscriptions", new JsonContent(json));
    }

    public override void Dispose()
    {
        MainSocket.Dispose();
        BackSocket?.Dispose();

        base.Dispose();
    }
}