using Scripts.ScriptableObjects;
using Scripts.UI.RoomUI;
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
                
            }
        }
    }
}
