using Scripts.Map.Room;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{
    public class MapManager : MonoBehaviour
    {
        private List<List<short>> _mapPathfinding = new List<List<short>>();
        private List<ARoom> _mapRooms = new List<ARoom>();

        [SerializeField]
        private GameObject _rooms;

        public void AddRoom(Vector2Int _position, GameObject room, RoomType type)
        {
            var size = new Vector2Int(Mathf.RoundToInt(room.transform.localScale.x), Mathf.RoundToInt(room.transform.localScale.y));
            _mapRooms.Add(new Corridor(size, _position));
        }
    }
}
