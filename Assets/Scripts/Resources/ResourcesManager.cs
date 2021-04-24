using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Resources
{
    public class ResourcesManager : MonoBehaviour
    {
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
    }
}
