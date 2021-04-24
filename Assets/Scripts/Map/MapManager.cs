using Scripts.Map.Room;
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
        private GameObject[] _rooms;
        [SerializeField]
        private GameObject _corridor;
        [SerializeField]
        private GameObject _elevator;

        private void Start()
        {
            // Base zone discovered
            const int baseDiscoveredSize = 20;
            for (int y = 0; y < baseDiscoveredSize; y++)
            {
                List<TileState> _elems = new List<TileState>();
                for (int x = 0; x < baseDiscoveredSize; x++)
                {
                    _elems.Add(TileState.EMPTY);
                    _debugExploration.Add((x, y, Color.white));
                }
                _mapPathfinding.Add(_elems);
            }
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

            var offset = new Vector2(1f, 0f);
            var go = Instantiate(room, _position + offset, Quaternion.identity);
            go.transform.parent = _mapTransform;

            for (int y = _position.y; y < _position.y + size.y; y++)
            {
                for (int x = _position.x; x < _position.x + size.x; x++)
                {
                    SetTileStatus(x, y, TileState.OCCUPIED);
                }
            }
        }

        private List<(int, int, Color)> _debugExploration = new List<(int, int, Color)>();
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
