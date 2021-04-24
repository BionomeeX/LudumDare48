using Scripts.Map.Room;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{
    public class MapManager : MonoBehaviour
    {
        private List<List<PathNode>> _mapPathfinding = new List<List<PathNode>>();
        private List<ARoom> _mapRooms = new List<ARoom>();

        [Header("Rooms")]
        [SerializeField]
        private GameObject _receptionRoom;
        [SerializeField]
        private GameObject _rooms;
        [SerializeField]
        private GameObject _corridors;

        private void Start()
        {
            AddRoom(new Vector2Int(5, 0), _receptionRoom, RoomType.RECEPTION);
        }

        public void AddRoom(Vector2Int _position, GameObject room, RoomType type)
        {
            var size = new Vector2Int(Mathf.RoundToInt(room.transform.localScale.x), Mathf.RoundToInt(room.transform.localScale.y));
            switch (type)
            {
                case RoomType.RECEPTION:
                    _mapRooms.Add(new ReceptionRoom(size, _position));
                    break;

                default:
                    throw new System.ArgumentException("Invalid room type " + type.ToString(), nameof(type));
            }

            // TODO: Fill _mapPathfinding from _position to _position + size
        }
    }
}
