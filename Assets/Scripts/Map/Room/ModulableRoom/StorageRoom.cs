using Scripts.Resources;
using Scripts.ScriptableObjects;
using Scripts.UI.RoomUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.Map.Room.ModulableRoom
{
    public class StorageRoom : AModulableRoom
    {
        public StorageRoom(GenericRoom r) : base()
        {
            Stock = new ResourceStock(r, ConfigManager.S.Config.NormalStorageMaxSize);
        }

        public override string GetName()
            => "Storage";

        public override string GetDescription()
            => "All your materials are stored here, without it you won't be able to mine or build anything";

        public override GameObject GetDescriptionPanel()
            => UIRoom.S._uiStorage;

        public void IncreaseResource(ResourceType rType, Text priorityText)
        {
            priorityText.text = Stock.SetPriority(rType, 1);
        }

        public void DecreaseResource(ResourceType rType, Text priorityText)
        {
            priorityText.text = Stock.SetPriority(rType, -1);
        }

        public override void SetupConfigPanel(GameObject go)
        {
            var c = go.GetComponent<StorageUI>();
            c.StorageInfoText.text = $"Space Taken: {Stock.GetSizeOccupiedWithReservation()} / {Stock.MaxSize}";
            for (int i = 0; i < c.PriorityContainer.transform.childCount; i++)
            {
                Object.Destroy(c.PriorityContainer.transform.GetChild(i));
            }
            Dictionary<ResourceType, (int, int)> _allResources = new Dictionary<ResourceType, (int, int)>();
            foreach (var r in ResourcesManager.S.GetKnownResources())
            {
                _allResources.Add(r, (0, 0));
            }
            foreach (var r in Stock._resources)
            {
                _allResources.Add(r.Key, (r.Value, 0));
            }
            foreach (var r in Stock._reserved)
            {
                _allResources.Add(r.Value.Item1, (r.Value.Item2, r.Value.Item2));
            }
            int ci = 0;
            foreach (var r in _allResources)
            {
                var parent = c.PriorityContainer.transform;
                var rT = (RectTransform)parent.transform;
                var g = Object.Instantiate(c.PriorityPrefab, parent);
                var gC = g.GetComponent<StoragePriorityUI>();
                gC.Name.text = $"{r.Key} - {r.Value.Item1} ({r.Value.Item1 - r.Value.Item2})";
                ((RectTransform)g.transform).position = new Vector3(rT.position.x, rT.position.y - (25f * ci), rT.position.z);
                var curr = r.Key;
                gC.Up.onClick.AddListener(new UnityAction(() => { IncreaseResource(curr, gC.PriorityText); }));
                gC.Down.onClick.AddListener(new UnityAction(() => { DecreaseResource(curr, gC.PriorityText); }));
                ci++;
            }
        }
    }
}
