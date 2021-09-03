﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using JoySoftware.HomeAssistant.Client;
using JoySoftware.HomeAssistant.Model;

namespace NetDaemon.Model3.Internal
{
    [SuppressMessage("", "CA1812", Justification = "Is Loaded via DependencyInjection")]
    internal class EntityStateCache
    {
        private readonly IHassClient _hassClient;

        public EntityStateCache(IHassClient hassClient)
        {
            _hassClient = hassClient;
        }

        private readonly ConcurrentDictionary<string, HassState?> _latestStates = new();

        public void Store(HassState newState) => _latestStates[newState.EntityId] = newState;

        public HassState? GetState(string entityId)
        {
            // Load missing states on demand,
            // this is a blocking call if it is not present but we want to avoid making GetState async

            return _latestStates.GetOrAdd(entityId, _ => _hassClient.GetState(entityId).Result);
        }
    }
}