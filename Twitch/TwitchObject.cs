using System;

namespace Twitch;

public abstract class TwitchObject(string clientID) : IDisposable
{
    protected readonly string clientID = clientID;
    protected bool disposed = false;

    public abstract void Update(float deltaTime);

    public virtual void Dispose()
    {
        disposed = true;
        System.GC.SuppressFinalize(this);
    }
}
