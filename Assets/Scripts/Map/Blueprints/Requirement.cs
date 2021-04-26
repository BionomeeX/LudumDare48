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
        private ARoom _room;

        public Requirement(ARoom room, ResourceInfo[] resources)
        {
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
                } while (amount > 0);
            }
        }

        public ReadOnlyCollection<(ResourceType type, int amount)> GetRequirements()
            => _waiting.AsReadOnly();

        /// <summary>
        /// Reserve a requirement
        /// </summary>
        /// <param name="id">ID of the agent that did the reservation</param>
        /// <param name="index">Index of the element in the requirements</param>
        public void Reserve(int id, int index)
        {
            var elem = _waiting[index];
            _waiting.RemoveAt(index);
            _reserved.Add(id, elem);
        }

        public void CompleteReservation(int id)
        {
            if (!_reserved.ContainsKey(id))
            {
                throw new ArgumentException("There is no resource reserved for " + id, nameof(id));
            }
            _reserved.Remove(id);
            if (_waiting.Count == 0 && _reserved.Count == 0) // Room created
            {
                EventManager.S.NotifyManager(Event.BlueprintFinished, _room);
                MapManager.S.BuildRoomExt(_room);
                // EventManager.S.NotifyManager(Events.Event.BlueprintFinished, _room);
            }
            OpenGameMenu.S.UpdateRoomInfo();
        }

        public void CancelReservation(int id)
        {
            if (_reserved.ContainsKey(id))
            {
                var elem = _reserved[id];
                _waiting.Add((elem.Item1, elem.Item2));
                _reserved.Remove(id);
            }
        }

        List<(ResourceType, int)> _waiting = new List<(ResourceType, int)>();
        Dictionary<int, (ResourceType, int)> _reserved = new Dictionary<int, (ResourceType, int)>();

        public (ResourceType, int)[] GetMissingResources()
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

        public void ResetAll()
        {
            foreach (var r in _reserved)
            {
                _waiting.Add((r.Value.Item1, r.Value.Item2));
            }
            _reserved = new Dictionary<int, (ResourceType, int)>();
        }
    }
}
