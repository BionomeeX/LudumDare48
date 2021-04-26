﻿using Scripts.Map.Room;
using Scripts.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        private List<ARoom> _miningRooms = new List<ARoom>();

        [SerializeField]
        private GameObject _metalPrefab;

        public Metal[] GetMetalsCloseEnough(Vector2 me)
            => _metals.Where(x => Vector2.Distance(x.transform.position, me) < ConfigManager.S.Config.MiningRange).ToArray();

        public void UpdateRoom(GenericRoom room)
        {
            if (room.RoomType.IsMining())
            {
                _miningRooms.Add(room);
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
        }

        private void Start()
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (var m in ConfigManager.S.Config.MiningResources)
                {
                    if (m.LayerFrom >= i && m.LayerTo <= i)
                    {
                        var amount = Random.Range(ConfigManager.S.Config.MinMetalPerLayer, ConfigManager.S.Config.MaxMetalPerLayer);
                        var go = Instantiate(_metalPrefab, new Vector2(0f,
                            Random.Range(i * ConfigManager.S.Config.LayerYSize,
                            (i + 1) * ConfigManager.S.Config.LayerYSize)), Quaternion.identity);
                        go.GetComponent<MeshRenderer>().material = m.Material;
                        _metals.Add(go.GetComponent<Metal>());
                    }
                }
            }
        }
    }
}
