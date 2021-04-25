using Scripts.Map.Blueprints;
using Scripts.Map.Room;
using Scripts.Map.Room.ModulableRoom;
using Scripts.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Scripts.Events;
using Scripts.Agents;



namespace Scripts.Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager S;

        private void Awake()
        {
            S = this;
        }

        private List<List<TileState>> _mapPathfinding = new List<List<TileState>>();
        public List<ARoom> MapRooms = new List<ARoom>();
        public List<MasterBlueprint> MapMasterBlueprints = new List<MasterBlueprint>();

        [Header("Rooms")]
        [SerializeField]
        private Transform _mapTransform;

        [SerializeField]
        public GameObject ReceptionRoom;
        [SerializeField]
        public RoomInfo[] Rooms;
        [SerializeField]
        public GameObject Corridor;
        [SerializeField]
        private GameObject _elevator;

        [Header("Other")]
        [SerializeField]
        private GameObject _constructionSign;

        [SerializeField]
        private GameObject _blueprintSign;

        [SerializeField]
        private GameObject _commandantPrefab;

        [SerializeField]
        private GameObject _warehousePrefab;


        private EntryZone _entry = new EntryZone();

        [SerializeField]
        private GameObject _debugText;

        private void Start()
        {
            // Base zone discovered
            const int baseDiscoveredSize = 80;
            for (int y = 0; y < baseDiscoveredSize; y++)
            {
                List<TileState> _elems = new List<TileState>();
                for (int x = 0; x < baseDiscoveredSize; x++)
                {
                    _elems.Add(TileState.EMPTY);
                }
                _mapPathfinding.Add(_elems);
            }

            ARoom firstRoom = AddRoom(new Vector2Int(5, 0), new Vector2Int(2, 1), ReceptionRoom, RoomType.RECEPTION, _entry, (r) =>
            {
                foreach (var res in ConfigManager.S.Config.StartingResources)
                {
                    ((GenericRoom)r).RoomType.Stock.AddResource(res.Type, res.Amount);

                }

                var warehouseman = Instantiate(_warehousePrefab, r.GameObject.transform.position + Vector3.up * .5f, Quaternion.identity);
                EventManager.S.Subscribe(warehouseman.GetComponent<Warehouseman>());

                var commandant = Instantiate(_commandantPrefab, r.GameObject.transform.position + Vector3.up * .5f, Quaternion.identity);
                EventManager.S.Subscribe(commandant.GetComponent<Commandant>());
            }, false);

            ARoom secondRoom = AddRoom(new Vector2Int(7, 0), new Vector2Int(2, 1), ReceptionRoom, RoomType.EMPTY, firstRoom, null, false);

            ARoom thirdRoom = AddRoom(new Vector2Int(9, 0), new Vector2Int(2, 1), ReceptionRoom, RoomType.EMPTY, secondRoom, null, false);

            ARoom fourthRoom = AddRoom(new Vector2Int(9, 1), new Vector2Int(2, 1), ReceptionRoom, RoomType.EMPTY, thirdRoom, null, false);



        }

        public bool CanIBuildHere(Vector2Int position, Vector2Int size)
        {
            if (position.x < 0 || position.y - size.y < 0)
            {
                return false;
            }
            for (int line = 0; line < size.y; ++line)
            {
                for (int col = 0; col < size.x; ++col)
                {
                    if (_mapPathfinding[position.x + line][position.y + col] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<ARoom> GetAllTurrets()
        {
            List<ARoom> result = new List<ARoom>();
            return result;
        }

        public List<ARoom> GetAllFactory()
        {
            List<ARoom> result = new List<ARoom>();
            return result;
        }

        public List<ARoom> GetAllAccessibleBlueprint()
        {
            var result = MapRooms.Where(
                r => r.Requirement != null
            ).Where(
                // check that at least one neighbor is not a blueprint
                r => r.GetNeighborhood().ToList().Where(nr => nr.Requirement == null).Count() > 0
            ).ToList();

            return result;
        }

        public List<ARoom> GetAllStockRoom()
        {
            List<ARoom> result = MapRooms.Where(
                r => r is GenericRoom gRoom && gRoom.RoomType.Stock != null
            ).ToList();
            return result;
        }

        public ARoom AddRoom(
            Vector2Int position,
            Vector2Int size,
            GameObject room,
            RoomType type,
            ARoom parentRoom,
            Action<ARoom> roomBuilt,
            bool hasCommandant
        )
        {
            ARoom newRoom;
            switch (type)
            {
                case RoomType.RECEPTION:
                case RoomType.EMPTY:
                    var gRoom = new GenericRoom(size, position);
                    if (type == RoomType.RECEPTION)
                    {
                        gRoom.RoomType = new ReceptionRoom(gRoom);
                        gRoom.RoomType.Stock.Room = gRoom;
                    }
                    newRoom = gRoom;
                    break;

                case RoomType.CORRIDOR:
                    newRoom = new Corridor(size, position);
                    break;

                default:
                    throw new ArgumentException("Invalid room type " + type.ToString(), nameof(type));
            }
            MapRooms.Add(newRoom);

            var go = Instantiate(room, new Vector2(position.x, -position.y) + new Vector2(1f, -size.y), Quaternion.identity);
            go.transform.parent = _mapTransform;
            newRoom.GameObject = go;

            if (Application.isEditor) // Editor debug
            {
                var dText = Instantiate(_debugText, go.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                dText.GetComponent<TextMesh>().text = "(" + newRoom.Position.x + ", " + newRoom.Position.y + ")";
            }

            for (int y = position.y; y < position.y + size.y; y++)
            {
                for (int x = position.x; x < position.x + size.x; x++)
                {
                    _mapPathfinding[y][x] = TileState.OCCUPIED;
                }
            }

            // check where the parent room come from
            if (position.x >= parentRoom.Position.x + parentRoom.Size.x)
            // Comming from left
            {
                parentRoom.RoomRight = newRoom;
                newRoom.RoomLeft = parentRoom;
            }
            else if (position.x + size.x <= parentRoom.Position.x)
            // Comming from right
            {
                parentRoom.RoomLeft = newRoom;
                newRoom.RoomRight = parentRoom;
            }

            // Special case for Y => Only 1x1 connection from top <-> bottom from now
            else if (position.y >= parentRoom.Position.y)
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

            if (!hasCommandant)
            {
                StartCoroutine(BuildRoom(newRoom, roomBuilt, newRoom));
            }
            else
            {
                newRoom.Sign =
                    Instantiate(
                        _blueprintSign,
                        (Vector3)(new Vector2(newRoom.Position.x, -newRoom.Position.y))
                        + new Vector3(newRoom.Size.x / 2f, -newRoom.Size.y / 2f, -1f),
                        Quaternion.identity);
                List<ResourceInfo> allRequirements = new List<ResourceInfo>();
                int nbBlocks = newRoom.Size.x * newRoom.Size.y;
                for (int i = 0; i < nbBlocks; i++)
                {
                    allRequirements.AddRange(ConfigManager.S.Config.RequirementPerBloc);
                }
                newRoom.Requirement = new Requirement(Enumerable.Repeat(ConfigManager.S.Config.RequirementPerBloc, newRoom.Size.x * newRoom.Size.y).SelectMany(x => x).ToArray());
            }

            return newRoom;
        }

        private IEnumerator BuildRoom(ARoom room, Action<ARoom> roomBuilt, ARoom newRoom)
        {
            var sign = Instantiate(_constructionSign, (Vector3)(new Vector2(room.Position.x, -room.Position.y)) + new Vector3(room.Size.x / 2, -room.Size.y / 2f, -1f), Quaternion.identity);
            yield return new WaitForSeconds(3f);
            room.IsBuilt = true;
            Destroy(sign);
            roomBuilt?.Invoke(newRoom);
        }

        public void DiscoverTile(int x, int y)
        {
            for (int i = _mapPathfinding.Count; i <= y; i++)
            {
                _mapPathfinding.Add(new List<TileState>());
            }
            var list = _mapPathfinding[y];
            for (int i = list.Count; i <= x; i++)
            {
                list.Add(TileState.NOT_DISCOVERED);
            }
            if (_mapPathfinding[y][x] == TileState.NOT_DISCOVERED)
            {
                _mapPathfinding[y][x] = TileState.EMPTY;
            }
        }

        private void OnDrawGizmos()
        {
            for (int y = 0; y < _mapPathfinding.Count; y++)
            {
                for (int x = 0; x < _mapPathfinding[y].Count; x++)
                {
                    var elem = _mapPathfinding[y][x];
                    if (elem == TileState.NOT_DISCOVERED)
                    {
                        continue;
                    }
                    Gizmos.color = elem == TileState.EMPTY ? Color.white : Color.red;
                    Gizmos.DrawLine(new Vector2(x, -y), new Vector2(x, -y - 1f));
                    Gizmos.DrawLine(new Vector2(x, -y), new Vector2(x + 1f, -y));
                    Gizmos.DrawLine(new Vector2(x + 1f, -y), new Vector2(x + 1f, -y - 1f));
                    Gizmos.DrawLine(new Vector2(x, -y - 1f), new Vector2(x + 1f, -y - 1f));
                }
            }
        }
    }
}
