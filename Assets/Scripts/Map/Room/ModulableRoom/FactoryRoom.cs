using Scripts.Craft;
using Scripts.Map.Blueprints;
using Scripts.ScriptableObjects;
using Scripts.UI;
using Scripts.UI.RoomUI;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Map.Room.ModulableRoom
{
    public class FactoryRoom : AModulableRoom
    {
        public FactoryRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.FactoryStorageMaxSize);
            Formula = ConfigManager.S.Config.Formulas[0];
            req = new Requirement(r, Formula.Input.Select(x => new ResourceInfo()
            {
                Type = x,
                Amount = ConfigManager.S.Config.NbOfResourcePerTransportation
            }).ToArray(), true);
            r.Requirement = req;
        }

        public override string GetName()
            => "Factory";

        public override string GetDescription()
            => "Convert your ores into refined resources";

        public override GameObject GetDescriptionPanel()
            => UIRoom.S._factoryStorage;

        public Formula Formula;
        private Requirement req;
        public override void SetupConfigPanel(GameObject go)
        {
            var c = go.GetComponent<FactoryUI>();
            c.StorageInfoText.text = $"Space Taken: {Stock.GetSizeOccupiedWithReservation()} / {Stock.MaxSize}\nCrafting {Formula.Output}";
            c.BAmmo.onClick.AddListener(new UnityAction(() =>
            {
                Formula = ConfigManager.S.Config.Formulas[0];
                UpdateReq();
                OpenGameMenu.S.UpdateRoomInfo();
            }));
            c.BSteel.onClick.AddListener(new UnityAction(() =>
            {
                Formula = ConfigManager.S.Config.Formulas[1];
                UpdateReq();
                OpenGameMenu.S.UpdateRoomInfo();
            }));
        }

        private void UpdateReq()
        {
            req = new Requirement(req._room, Formula.Input.Select(x => new ResourceInfo()
            {
                Type = x,
                Amount = ConfigManager.S.Config.NbOfResourcePerTransportation
            }).ToArray(), true);
            req._room.Requirement = req;
        }
    }
}
