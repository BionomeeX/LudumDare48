using Scripts.Map.Room;
using Scripts.Map.Room.ModulableRoom;
using Scripts.Resources;
using Scripts.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Scripts.Events;

namespace Scripts.Extraction
{
    public class MiningManager : MonoBehaviour
    {
        public static MiningManager S;

        private void Awake()
        {
            S = this;
        }

        private List<Metal> _metals = new List<Metal>();

        private List<GenericRoom> _miningRooms = new List<GenericRoom>();

        [SerializeField]
        private GameObject _metalPrefab;

        public Metal[] GetMetalsCloseEnough(Vector2 me)
            => _metals.Where(x => Vector2.Distance(x.transform.position, me) < ConfigManager.S.Config.MiningRange).ToArray();

        public void UpdateRoom(GenericRoom room)
        {
            if (room.RoomType.IsMining())
            {
                var metals = GetMetalsCloseEnough(room.GameObject.transform.position);
                if (metals.Length > 0)
                {
                    var mr = (MiningRoom)room.RoomType;
                    mr.CurrentMining = metals[0];
                    mr.Possibilities = metals;
                }
                _miningRooms.Add(room);
            }
        }

        int timer = 0;
        private void FixedUpdate()
        {
            timer++;
            if (timer == ConfigManager.S.Config.MiningPerFixedUpdate)
            {
                foreach (var r in _miningRooms)
                {
                    if (r.RoomType.Stock.GetSizeTakenWithReservation() < r.RoomType.Stock.MaxSize)
                    {
                        r.RoomType.Stock.AddResource(((MiningRoom)r.RoomType).CurrentMining.Type, 1);
                        EventManager.S.NotifyManager(Events.Event.ResourceMined, this);
                    }
                }
                ResourcesManager.S.UpdateUI();
                timer = 0;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var room in _miningRooms)
            {
                foreach (var link in GetMetalsCloseEnough(room.GameObject.transform.position))
                {
                    Gizmos.DrawLine(room.GameObject.transform.position, link.transform.position);
                }
            }
            Gizmos.color = Color.red;
            foreach (var room in _miningRooms)
            {
                var mRoom = (MiningRoom)room.RoomType;
                if (mRoom.CurrentMining != null)
                {
                    Gizmos.DrawLine(room.GameObject.transform.position, mRoom.CurrentMining.transform.position);
                }
            }
        }

        private void Start()
        {
            var metalParent = new GameObject("Metals");
            for (int i = 0; i < 4; i++)
            {
                foreach (var m in ConfigManager.S.Config.MiningResources)
                {
                    if (i >= m.LayerFrom && i <= m.LayerTo)
                    {
                        var amount = Random.Range(ConfigManager.S.Config.MinMetalPerLayer, ConfigManager.S.Config.MaxMetalPerLayer);
                        var go = Instantiate(_metalPrefab, new Vector2(0f,
                            -Random.Range((float)i * ConfigManager.S.Config.LayerYSize,
                            (i + 1) * ConfigManager.S.Config.LayerYSize)), Random.rotation);
                        go.transform.parent = metalParent.transform;
                        go.GetComponent<MeshRenderer>().material = m.Material;
                        var metal = go.GetComponent<Metal>();
                        metal.Type = m.Resource;
                        _metals.Add(metal);
                    }
                }
            }
        }
    }
}
