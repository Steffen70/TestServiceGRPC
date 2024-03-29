﻿using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TestServiceGRPC.Utils.Extensions;
using TestServiceGRPC.Utils.Services;

namespace TestServiceGRPC.Hubs;

[Authorize]
public class SessionPersistenceHub<TSessionData> : Hub where TSessionData : class, new()
{
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> ConnectionsByRefId = new();

    private readonly SessionService<TSessionData> _sessionService;
    private readonly RefGuidService _dataReference;

    private Guid RefGuid => _dataReference.Value!.Value;

    public SessionPersistenceHub(IServiceProvider serviceProvider)
    {
        serviceProvider = serviceProvider.GetSessionScope();

        _sessionService = serviceProvider.GetRequiredService<SessionService<TSessionData>>();
        _dataReference = serviceProvider.GetRequiredService<RefGuidService>();
    }

    public override async Task OnConnectedAsync()
    {
        ConnectionsByRefId.AddOrUpdate(RefGuid,
            new HashSet<string> { Context.ConnectionId },
            (key, oldValue) => { oldValue.Add(Context.ConnectionId); return oldValue; });

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($@"Connection added: {Context.ConnectionId}");
        Console.ForegroundColor = ConsoleColor.White;

        _sessionService.ReleaseData();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($@"Session data released: {RefGuid}");
        Console.ForegroundColor = ConsoleColor.White;

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ConnectionsByRefId.TryGetValue(RefGuid, out var connections))
        {
            connections.Remove(Context.ConnectionId);
            if (connections.Count == 0)
                ConnectionsByRefId.TryRemove(RefGuid, out _);

            await _sessionService.RemoveDataFromCacheAsync();
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($@"Connection removed: {Context.ConnectionId}");
        Console.ForegroundColor = ConsoleColor.White;

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message)
    {
        if (ConnectionsByRefId.TryGetValue(RefGuid, out var connections))
            foreach (var connectionId in connections)
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
    }
}