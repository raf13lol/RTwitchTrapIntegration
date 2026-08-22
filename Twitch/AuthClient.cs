using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RTwitchTrapIntegration;
using Twitch.Models.Auth;

namespace Twitch;

public class AuthClient : TwitchObject
{
    private const string SCOPES = "channel:manage:redemptions";
    private const string TOKEN_GRANT_TYPE = "urn:ietf:params:oauth:grant-type:device_code";
    private const string BASE_API_URL = "https://api.twitch.tv/helix/";

    public HttpClient WebClient;

    private string deviceCode;

    private float currentExpiryPassed;
    private float expiryTime;

    public string AccessToken;
    public string RefreshToken;

    public delegate void DeviceCodeReceivedCallback(DeviceCodeObject deviceCode);
    public delegate void AccessTokenReceivedCallback(AuthClient auth);

    public event DeviceCodeReceivedCallback OnDeviceCodeReceived;
    public event AccessTokenReceivedCallback OnAccessTokenReceived;

    public AuthClient(string clientID) : base(clientID)
    {
        WebClient = new();
    }

    public override void Update(float deltaTime)
    {
        if (string.IsNullOrEmpty(deviceCode) || disposed)
            return;
            
        if (currentExpiryPassed > expiryTime)
        {
            deviceCode = string.Empty;
            _ = CreateDeviceAuthorisationRequest();
        }
        currentExpiryPassed += deltaTime;
    }

    public async Task<DeviceCodeObject> CreateDeviceAuthorisationRequest()
    {
        string res = await MultipartFormRequest("https://id.twitch.tv/oauth2/device", new()
        {
            ["client_id"] = clientID,
            ["scopes"] = SCOPES
        });

        DeviceCodeObject deviceObject = JsonConvert.DeserializeObject<DeviceCodeObject>(res);
        OnDeviceCodeReceived?.Invoke(deviceObject);

        deviceCode = deviceObject.device_code;
        currentExpiryPassed = 0f;
        expiryTime = deviceObject.expires_in;

        return deviceObject;
    }

    public async Task<bool> TryUseDeviceCode()
    {
        if (string.IsNullOrEmpty(deviceCode))
            return false;

        string res = await MultipartFormRequest("https://id.twitch.tv/oauth2/token", new()
        {
            ["client_id"] = clientID,
            ["scopes"] = SCOPES,
            ["device_code"] = deviceCode,
            ["grant_type"] = TOKEN_GRANT_TYPE
        });

        GrantTokenObject tokenObject = JsonConvert.DeserializeObject<GrantTokenObject>(res);
        if (tokenObject.status != 0)
        {
            Patch.Log.LogMessage($"Getting access token failed with message: '{tokenObject.message}' (status {tokenObject.status})");
            return false;
        }

        AccessToken = tokenObject.access_token;
        RefreshToken = tokenObject.refresh_token;
        deviceCode = string.Empty;

        OnAccessTokenReceived?.Invoke(this);
        return true;
    }

    public async Task<string> MakeRequestQuery(HttpMethod method, string url, Dictionary<string, string> queries, HttpContent content = null)
    {
        bool firstKvp = true;
        foreach (KeyValuePair<string, string> kvp in queries)
        {
            char prefixCharacter = firstKvp ? '?' : '&';

            string key = Uri.EscapeDataString(kvp.Key);
            string value = Uri.EscapeDataString(kvp.Value);

            url += $"{prefixCharacter}{key}={value}";
            firstKvp = false;
        }
        
        return await MakeRequest(method, url, content);
    }

    public async Task<string> MakeRequest(HttpMethod method, string url, HttpContent content = null)
    {
        if (string.IsNullOrEmpty(AccessToken))
            return "";

        using HttpRequestMessage request = new(method, new Uri(BASE_API_URL + url));
        request.Headers.Authorization = new("Bearer", AccessToken);
        request.Headers.Add("Client-Id", clientID);

        if (content != null)
            request.Content = content;

        HttpResponseMessage message = await WebClient.SendAsync(request);
        if (message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // try refresh our token right now
            message.Dispose();

            using HttpRequestMessage tokenRequest = new(HttpMethod.Post, new Uri("https://id.twitch.tv/oauth2/token"));
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                ["client_id"] = clientID,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = RefreshToken
            });

            using HttpResponseMessage tokenResponse = await WebClient.SendAsync(tokenRequest);

            string res = await tokenResponse.Content.ReadAsStringAsync();
            GrantTokenObject tokenObject = JsonConvert.DeserializeObject<GrantTokenObject>(res);

            if (tokenObject.status > 0)
            {
                // Fuck
                AccessToken = string.Empty;
                RefreshToken = string.Empty;
                return string.Empty;
            }

            AccessToken = tokenObject.access_token;
            RefreshToken = tokenObject.refresh_token;

            // resend
            request.Headers.Authorization = new("Bearer", AccessToken);
            message = await WebClient.SendAsync(request);
        }

        string messageContent = await message.Content.ReadAsStringAsync();
        message.Dispose();
        
        return messageContent;
    }

    private async Task<string> MultipartFormRequest(string url, Dictionary<string, string> data)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, new Uri(url));

        MultipartFormDataContent content = [];

        foreach (KeyValuePair<string, string> kvp in data)
            content.Add(new StringContent(kvp.Value), kvp.Key);
        request.Content = content;

        using HttpResponseMessage message = await WebClient.SendAsync(request);
        return await message.Content.ReadAsStringAsync();
    }

    public override void Dispose()
    {
        WebClient.Dispose();
        base.Dispose();
    }
}