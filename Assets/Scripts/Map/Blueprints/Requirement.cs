using Scripts.Resources;
using Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scripts.Map.Blueprints
{
    public class Requirement
    {
        public Requirement(ResourceInfo[] resources)
        {

        }

        public ReadOnlyCollection<(ResourceType, int)> GetRequirements()
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
    }
}
