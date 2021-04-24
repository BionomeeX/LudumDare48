using Scripts.Map.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{
    public class MapManager : MonoBehaviour
    {
        private List<List<TileState>> _mapPathfinding = new List<List<TileState>>();
        private List<ARoom> _mapRooms = new List<ARoom>();

        [Header("Rooms")]
        [SerializeField]
        private Transform _mapTransform;

        [SerializeField]
        private GameObject _receptionRoom;
        [SerializeField]
        private RoomInfo[] _rooms;
        [SerializeField]
        private GameObject _corridor;
        [SerializeField]
        private GameObject _elevator;

        [Header("Other")]
        [SerializeField]
        private GameObject _constructionSign;

        private EntryZone _entry = new EntryZone();

        private void Start()
        {
            // Base zone discovered
            const int baseDiscoveredSize = 20;
            for (int y = 0; y < baseDiscoveredSize; y++)
            {
                List<TileState> _elems = new List<TileState>();
                List<ARoom> _elements = new List<ARoom>();
                for (int x = 0; x < baseDiscoveredSize; x++)
                {
                    _elems.Add(TileState.EMPTY);
                    _debugExploration.Add((x, y, Color.white));
                }
                _mapPathfinding.Add(_elems);
            }

            ARoom firstRoom = AddRoom(new Vector2Int(5, 0), new Vector2Int(2, 1), _receptionRoom, RoomType.RECEPTION, _entry);

            ARoom secondRoom = AddRoom(new Vector2Int(7, 0), new Vector2Int(2, 1), _receptionRoom, RoomType.RECEPTION, firstRoom);

            ARoom thirdRoom = AddRoom(new Vector2Int(9, 0), new Vector2Int(2, 1), _receptionRoom, RoomType.RECEPTION, secondRoom);

            ARoom fourthRoom = AddRoom(new Vector2Int(9, 1), new Vector2Int(2, 1), _receptionRoom, RoomType.RECEPTION, thirdRoom);

            var roompath = Astar.FindPath(firstRoom, fourthRoom);

            Debug.Log(roompath.Count);
            foreach(var room in roompath){
                Debug.Log(room.Position.x + " " + room.Position.y);
            }

        }

        public ARoom AddRoom(Vector2Int position, Vector2Int size, GameObject room, RoomType type, ARoom parentRoom)
        {
            ARoom newRoom;
            switch (type)
            {
                case RoomType.RECEPTION:
                    newRoom = new ReceptionRoom(size, position);
                    break;

                default:
                    throw new System.ArgumentException("Invalid room type " + type.ToString(), nameof(type));
            }
            _mapRooms.Add(newRoom);

            var go = Instantiate(room, position + new Vector2(1f, -size.y), Quaternion.identity);
            go.transform.parent = _mapTransform;

            for (int y = position.y; y < position.y + size.y; y++)
            {
                for (int x = position.x; x < position.x + size.x; x++)
                {
                    SetTileStatus(x, y, TileState.OCCUPIED);
                }
            }

            // check where the parent room come from
            if (position.x > parentRoom.Position.x + parentRoom.Size.x)
            // Comming from left
            {
                parentRoom.RoomRight = newRoom;
                newRoom.RoomLeft = parentRoom;
            }
            else if (position.x + size.x < parentRoom.Position.x)
            // Comming from right
            {
                parentRoom.RoomLeft = newRoom;
                newRoom.RoomRight = parentRoom;
            }

            // Special case for Y => Only 1x1 connection from top <-> bottom from now
            else if (position.y > parentRoom.Position.y)
            // Comming from bottom
            {
                parentRoom.RoomUp = newRoom;
                newRoom.RoomDown = parentRoom;
            }
            else
            // Comming from top
            {
                parentRoom.RoomDown = newRoom;
                newRoom.RoomUp = parentRoom;
            }

            StartCoroutine(BuildRoom(newRoom));

            return newRoom;
        }

        private IEnumerator BuildRoom(ARoom room)
        {
            var sign = Instantiate(_constructionSign, (Vector3)((Vector2)room.Position) + new Vector3(room.Size.x / 2, -room.Size.y / 2f, -1f), Quaternion.identity);
            yield return new WaitForSeconds(3f);
            room.IsBuilt = true;
            Destroy(sign);
        }

        private List<(int, int, Color)> _debugExploration = new List<(int, int, Color)>();
        private List<(Vector2Int, Vector2Int, Color)> _debugLinks = new List<(Vector2Int, Vector2Int, Color)>();
        private void SetTileStatus(int x, int y, TileState state)
        {
            _mapPathfinding[y][x] = state;
            switch (state)
            {
                case TileState.EMPTY:
                    _debugExploration.Add((x, y, Color.white));
                    break;

                case TileState.OCCUPIED:
                    _debugExploration.Add((x, y, Color.red));
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var d in _debugExploration)
            {
                Gizmos.color = d.Item3;
                var x = d.Item1;
                var y = -d.Item2;
                Gizmos.DrawLine(new Vector2(x, y), new Vector2(x, y - 1f));
                Gizmos.DrawLine(new Vector2(x, y), new Vector2(x + 1f, y));
                Gizmos.DrawLine(new Vector2(x + 1f, y), new Vector2(x + 1f, y - 1f));
                Gizmos.DrawLine(new Vector2(x, y - 1f), new Vector2(x + 1f, y - 1f));
            }
        }
    }
}
