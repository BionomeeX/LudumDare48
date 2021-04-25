using Scripts.Resources;
using Scripts.ScriptableObjects;
using Scripts.UI.RoomUI;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public class StorageRoom : AModulableRoom
    {
        public StorageRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.NormalStorageMaxSize);
        }

        public override string GetName()
            => "Storage";

        public override string GetDescription()
            => "All your materials are stored here, without it you won't be able to mine or build anything";

        public override GameObject GetDescriptionPanel()
            => UIRoom.S._uiStorage;

        public override void SetupConfigPanel(GameObject go)
        {
            var c = go.GetComponent<StorageUI>();
            c.StorageInfoText.text = $"Space Taken: {Stock.GetSizeTaken()} / {Stock.MaxSize}";
            for (int i = 0; i < c.PriorityContainer.transform.childCount; i++)
            {
                Object.Destroy(c.PriorityContainer.transform.GetChild(i));
            }
            Dictionary<ResourceType, (int, int)> _allResources = new Dictionary<ResourceType, (int, int)>();
            foreach (var r in Stock._resources)
            {
                if (_allResources.ContainsKey(r.Key)) _allResources[r.Key] = (_allResources[r.Key].Item1 + r.Value, _allResources[r.Key].Item2);
                else _allResources.Add(r.Key, (r.Value, 0));
            }
            foreach (var r in Stock._reserved)
            {
                if (_allResources.ContainsKey(r.Value.Item1)) _allResources[r.Value.Item1] = (_allResources[r.Value.Item1].Item1 + r.Value.Item2, _allResources[r.Value.Item1].Item2 + r.Value.Item2);
                else _allResources.Add(r.Value.Item1, (r.Value.Item2, r.Value.Item2));
            }
            int ci = 0;
            foreach (var r in _allResources)
            {
                var g = Object.Instantiate(c.PriorityContainer, c.PriorityContainer.transform);
                var gC = g.GetComponent<StoragePriorityUI>();
                gC.Name.text = $"{r.Key} - {r.Value.Item1} ({r.Value.Item1 - r.Value.Item2})";
                ((RectTransform)g.transform).position = new Vector3(0f, 25f * ci, 0f);
                ci++;
            }
        }
    }
}
