using Scripts.Extraction;
using Scripts.ScriptableObjects;
using Scripts.UI.RoomUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
            for (int i = 0; i <  c.ChangeMiningPanel.transform.childCount; i++)
            {
                Object.Destroy(c.ChangeMiningPanel.transform.GetChild(i).gameObject);
            }
            List<string> names = new List<string>();
            int i2 = 0;
            foreach (var p in Possibilities)
            {
                if (!names.Contains(p.Type.ToString()))
                {
                    var go2 = Object.Instantiate(c.Button, c.ChangeMiningPanel.transform);
                    ((RectTransform)go2.transform).position = c.ChangeMiningPanel.transform.position + new Vector3(0f, i2 * -30f, 0f);
                    go2.GetComponentInChildren<Text>().text = p.Type.ToString();
                    //go2.GetComponent<Button>().
                    names.Add(p.Type.ToString());
                    i2++;
                }
            }
        }

        public Metal CurrentMining;
        public Metal[] Possibilities;
    }
}
