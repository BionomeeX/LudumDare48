using Scripts.Events;
using Scripts.Map.Room;
using Scripts.Resources;
using Scripts.ScriptableObjects;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Scripts.Map.Blueprints
{
    public class Requirement
    {
        public ARoom _room;
        private bool _endlessMode;
        private Dictionary<ResourceType, int> _endlessReserve = new Dictionary<ResourceType, int>();

        public Requirement(ARoom room, ResourceInfo[] resources, bool isEndless)
        {
            _endlessMode = isEndless;
            _room = room;
            foreach (var r in resources)
            {
                int amount = r.Amount;
                do
                {
                    var nb = amount > ConfigManager.S.Config.NbOfResourcePerTransportation
                        ? ConfigManager.S.Config.NbOfResourcePerTransportation
                        : amount;
                    _waiting.Add((r.Type, nb));
                    amount -= nb;
                    if (_endlessMode && !_endlessReserve.ContainsKey(r.Type))
                    {
                        _endlessReserve.Add(r.Type, 0);
                    }
                } while (amount > 0);
            }
        }

        public ReadOnlyCollection<(ResourceType type, int amount)> GetRequirements()
            => !_endlessMode ? _waiting.AsReadOnly() : GetMissingResources().ToList().AsReadOnly();

        /// <summary>
        /// Reserve a requirement
        /// </summary>
        /// <param name="id">ID of the agent that did the reservation</param>
        /// <param name="index">Index of the element in the requirements</param>
        public void Reserve(int id, int index)
        {
            var elem = _waiting[index];
            if (!_endlessMode)
            {
                _waiting.RemoveAt(index);
            }
            _reserved.Add(id, elem);
        }

        public void CompleteReservation(int id)
        {
            if (!_reserved.ContainsKey(id))
            {
                throw new ArgumentException("There is no resource reserved for " + id, nameof(id));
            }
            var elem = _reserved[id];
            _reserved.Remove(id);
            if (!_endlessMode)
            {
                if (_waiting.Count == 0 && _reserved.Count == 0) // Room created
                {
                    EventManager.S.NotifyManager(Event.BlueprintFinished, _room);
                    MapManager.S.BuildRoomExt(_room);
                }
            }
            else
            {
                _endlessReserve.Add(elem.Item1, elem.Item2);
            }
            OpenGameMenu.S.UpdateRoomInfo();
        }

        public void CancelReservation(int id)
        {
            if (_reserved.ContainsKey(id))
            {
                var elem = _reserved[id];
                if (!_endlessMode)
                {
                    _waiting.Add((elem.Item1, elem.Item2));
                }
                _reserved.Remove(id);
            }
        }

        List<(ResourceType, int)> _waiting = new List<(ResourceType, int)>();
        Dictionary<int, (ResourceType, int)> _reserved = new Dictionary<int, (ResourceType, int)>();

        public int GetEndlessReserve()
            => _endlessReserve.Select(x => x.Value).Sum();

        public (ResourceType, int)[] GetMissingResources()
        {
            if (!_endlessMode)
            {
                Dictionary<ResourceType, int> allResources = new Dictionary<ResourceType, int>();
                foreach (var r in _waiting)
                {
                    if (allResources.ContainsKey(r.Item1)) allResources[r.Item1] += r.Item2;
                    else allResources.Add(r.Item1, r.Item2);
                }
                foreach (var r in _reserved.Values)
                {
                    if (allResources.ContainsKey(r.Item1)) allResources[r.Item1] += r.Item2;
                    else allResources.Add(r.Item1, r.Item2);
                }
                return allResources.Select(x => (x.Key, x.Value)).ToArray();
            }
            else
            {
                if (_endlessReserve.Select(x => x.Value).Sum() + 5 >= ConfigManager.S.Config.FactoryStorageMaxSize)
                {
                    return new (ResourceType, int)[0];
                }
                var required = _waiting.Select(x => x.Item1).Distinct().ToArray();
                ResourceType smaller = (ResourceType)(-1);
                int amount = int.MaxValue;
                foreach (var r in required)
                {
                    var e = _endlessReserve[r];
                    if (e < amount)
                    {
                        smaller = r;
                        amount = e;
                    }
                }
                return new[] { (smaller, ConfigManager.S.Config.NbOfResourcePerTransportation) };
            }
        }

        public void ResetAll()
        {
            if (!_endlessMode)
            {
                foreach (var r in _reserved)
                {
                    _waiting.Add((r.Value.Item1, r.Value.Item2));
                }
            }
            _reserved = new Dictionary<int, (ResourceType, int)>();
        }
    }
}
