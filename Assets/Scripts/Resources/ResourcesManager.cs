using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Resources
{
    public class ResourcesManager : MonoBehaviour
    {
        public static ResourcesManager S;

        private void Awake()
        {
            S = this;
        }

        [SerializeField]
        private Text _resourcesText;

        Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        private void Start()
        {
            _resources.Add(ResourceType.IRON, 100);
            UpdateUI();
        }

        private void UpdateUI()
        {
            _resourcesText.text = "Resources:\n" + string.Join("\n", _resources.Select(x => x.Key + ": " + x.Value));
        }

        public bool ConsumeResource(ResourceType type, int amount)
        {
            if (!_resources.ContainsKey(type) || _resources[type] < amount)
            {
                return false;
            }
            _resources[type] -= amount;
            return true;
        }
    }
}
