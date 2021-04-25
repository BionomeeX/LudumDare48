﻿using Scripts.Map.Room;
using System.Collections.Generic;

namespace Scripts.Resources
{
    public class ResourceStock
    {
        public ResourceStock(GenericRoom r)
        {
            Room = r;
        }

        public GenericRoom Room;

        // All resources available
        public Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        // Resources reserved by an agent
        // (agent id, type of resource, number of resource reserved)
        public List<(int, ResourceType, int)> _reserved = new List<(int, ResourceType, int)>();

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
        public int GetResources(ResourceType type)
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
            _reserved.Add((id, type, amount));
            _resources[type] -= amount;
        }
    }
}
