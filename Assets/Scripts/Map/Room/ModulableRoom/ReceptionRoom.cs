using Scripts.ScriptableObjects;
using Scripts.UI.RoomUI;
using System.Linq;
using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public class ReceptionRoom : AModulableRoom
    {
        public ReceptionRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.StartingResources.Select(x => x.Amount).Sum());
        }

        public override string GetName()
            => "Reception";

        public override string GetDescription()
            => "Your new agents will come here first";

        public override GameObject GetDescriptionPanel()
            => UIRoom.S._receptionStorage;

        public override void SetupConfigPanel(GameObject go)
        {
            var c = go.GetComponent<ReceptionUI>();
            c.StorageInfoText.text = $"Space Taken: {Stock.GetSizeTaken()} / {Stock.MaxSize}";
        }
    }
}
