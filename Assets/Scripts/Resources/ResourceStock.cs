using Scripts.Map.Room;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Resources
{
    public class ResourceStock
    {
        public ResourceStock(GenericRoom r, int maxSize)
        {
            Room = r;
            MaxSize = maxSize;
            ResourcesManager.S.AddStock(this);
        }

        public GenericRoom Room;
        public int MaxSize;

        public int GetSizeTaken()
            => _resources.Select(x => x.Value).Sum();

        public int GetSizeTakenWithReservation()
            => GetSizeTaken() + _reservedAdd.Select(x => x.Value.Item2).Sum();

        // All resources available
        public Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        // Resources reserved by an agent
        // (agent id, type of resource, number of resource reserved)
        public Dictionary<int, (ResourceType, int)> _reserved = new Dictionary<int, (ResourceType, int)>();
        public Dictionary<int, (ResourceType, int)> _reservedAdd = new Dictionary<int, (ResourceType, int)>();

        public void ReserveAddResource(int id, ResourceType type, int amount)
        {
            if (_reservedAdd.ContainsKey(id))
            {
                throw new InvalidOperationException(id + " already have a resource reserved here");
            }
            _reservedAdd.Add(id, (type, amount));
        }

        public void AddResourceFromReservation(int id)
        {
            if (!_reservedAdd.ContainsKey(id))
            {
                throw new ArgumentException("There is no resource reserved for " + id, nameof(id));
            }
            var elem = _reservedAdd[id];
            AddResource(elem.Item1, elem.Item2);
            _reservedAdd.Remove(id);
        }

        /// <summary>
        ///  Add a resource to the stock
        /// </summary>
        public void AddResource(ResourceType type, int amount)
        {
            if (_resources.ContainsKey(type))
            {
                _resources[type] += amount;
            }
            else
            {
                _resources.Add(type, amount);
            }
            ResourcesManager.S.UpdateUI();
        }

        /// <summary>
        /// Ask the stock if a resource is available
        /// </summary>
        /// <returns>The max amount of the resource that is available</returns>
        public int CheckResourceByType(ResourceType type)
        {
            if (!_resources.ContainsKey(type)) // Not available
            {
                return 0;
            }
            return _resources[type];
        }

        /// <summary>
        /// Reserve a resource for an id
        /// </summary>
        /// <remarks>You must call GetResources beforehand to be sure if there is enough resource!</remarks>
        public void ReserveResource(ResourceType type, int amount, int id)
        {
            if (_reserved.ContainsKey(id))
            {
                throw new InvalidOperationException(id + " already have a resource reserved here");
            }
            _reserved.Add(id, (type, amount));
            _resources[type] -= amount;
        }

        /// <summary>
        /// Called when an agent want to get a resource he previously reserved
        /// </summary>
        public (ResourceType resourceType, int amount) GetResource(int id)
        {
            if (!_reserved.ContainsKey(id))
            {
                throw new ArgumentException("There is no resource reserved for " + id, nameof(id));
            }
            var elem = _reserved[id];
            _reserved.Remove(id);
            return elem;
        }
    }
}
