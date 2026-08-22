using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using RTwitchTrapIntegration;
using Twitch.Models.EventSub.Messages;

namespace Twitch;

public class EventSubSocket() : IDisposable
{
    private const string WEBSOCKET_URL = "wss://eventsub.wss.twitch.tv/ws";

    public delegate void MessageReceivedCallback(EventSubMessage message);
    public event MessageReceivedCallback OnMessageReceived;

    public ClientWebSocket WebSocket = new();
    public string WebSocketID;

    public CancellationTokenSource CancellationTokenSource = new();
    public CancellationToken CancelToken => CancellationTokenSource.Token;
    public Task Thread;

    private bool disposed = false;
    private Memory<byte> receiveBuffer = new(new byte[1 << 18]);

    public async Task Connect(string reconnectURL = null)
    {
        await WebSocket.ConnectAsync(new(reconnectURL ?? WEBSOCKET_URL), CancelToken);

        EventSubMessage welcome = EventSubMessageParser.Parse(receiveBuffer, await WebSocket.ReceiveAsync(receiveBuffer, CancelToken));
        if (welcome.metadata.MessageType != EventSubMessageType.Welcome)
        {
            await WebSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "First message from server is not Welcome", CancelToken);
            return;
        }

        WebSocketID = welcome.payload.session.id;

        OnMessageReceived?.Invoke(welcome);
        Thread = Task.Run(() => ReceiveMessagesLoop(CancelToken), CancelToken);
    }

    public async Task ReceiveMessagesLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested && !disposed)
        {
            try
            {
                if (WebSocket.State != WebSocketState.Open)
                    throw new Exception("Dummy error to dispose this websocket");
                Patch.Log.LogMessage($"websock state = {WebSocket.State}");
                EventSubMessage message = EventSubMessageParser.Parse(receiveBuffer, await WebSocket.ReceiveAsync(receiveBuffer, token));
                OnMessageReceived?.Invoke(message);    
            }
            catch (Exception e)
            {
                Patch.Log.LogError(e.GetType().Name);
                Patch.Log.LogError(e.Message);
                Patch.Log.LogError(e.StackTrace);
                Dispose();
                break;
            }
        }
    }

    public void Dispose()
    {
        Patch.Log.LogMessage($"websock disposed");
        disposed = true;

        CancellationTokenSource.Dispose();
        CancellationTokenSource.Cancel();
        Thread?.Dispose();

        WebSocket.Dispose();

        System.GC.SuppressFinalize(this);
    }
}