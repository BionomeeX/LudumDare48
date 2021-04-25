using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        List<ResourceStock> _stocks = new List<ResourceStock>();

        private void Start()
        {
            UpdateUI();
        }

        public ReadOnlyCollection<ResourceStock> GetResourceStocks(Vector2 myPos)
            => _stocks.AsReadOnly();

        public void UpdateUI()
        {
            Dictionary<ResourceType, (int, int)> _allResources = new Dictionary<ResourceType, (int, int)>();
            foreach (var s in _stocks)
            {
                foreach (var r in s._resources)
                {
                    if (_allResources.ContainsKey(r.Key)) _allResources[r.Key] = (_allResources[r.Key].Item1 + r.Value, _allResources[r.Key].Item2);
                    else _allResources.Add(r.Key, (r.Value, 0));
                }
                foreach (var r in s._reserved)
                {
                    if (_allResources.ContainsKey(r.Item2)) _allResources[r.Item2] = (_allResources[r.Item2].Item1 + r.Item3, _allResources[r.Item2].Item2 + r.Item3);
                    else _allResources.Add(r.Item2, (r.Item3, r.Item3));
                }
            }
            _resourcesText.text = "Resources:\n" + string.Join("\n", _allResources.Select(x => $"{x.Key}: {x.Value.Item1} ({x.Value.Item2})"));
        }
    }
}
