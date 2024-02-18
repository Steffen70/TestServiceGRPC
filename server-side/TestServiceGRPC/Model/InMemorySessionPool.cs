using System.Collections.Concurrent;

namespace TestServiceGRPC.Model;

public class InMemorySessionPool<TSessionData>
{
    public readonly ConcurrentDictionary<Guid, SessionDataWrapper<TSessionData>> DataCache = new();

    public void TerminateSessions()
    {
        foreach (var valuePair in DataCache)
            if (valuePair.Value is { IsInitialized: true, Data: IDisposable disposable })
                disposable.Dispose();

        DataCache.Clear();
    }
}
