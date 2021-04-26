﻿using Scripts.Extraction;
using Scripts.ScriptableObjects;
using Scripts.UI.RoomUI;
using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public class MiningRoom : AModulableRoom
    {
        public MiningRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.MiningStorageMaxSize);
        }

        public override string GetName()
            => "Mining";

        public override string GetDescription()
            => "Gather materials for you, must be placed close to an ore source";

        public override bool IsMining()
            => true;

        public override GameObject GetDescriptionPanel()
            => UIRoom.S._miningStorage;

        public override void SetupConfigPanel(GameObject go)
        {
            var c = go.GetComponent<MiningUI>();
            c.StorageInfoText.text = $"Space Taken: {Stock.GetSizeOccupiedWithReservation()} / {Stock.MaxSize}\nCurrently mining {CurrentMining?.Type}";
        }

        public Metal CurrentMining;
    }
}
