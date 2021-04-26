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

        private List<(int width, int heigth)> _possibleRoomsSize = new List<(int width, int heigth)>{
                //(1, 1),
                (2, 1),
                (3, 1),
                (2, 2),
                (3, 2)
            };

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
        public GameObject Elevator;

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
            for (int y = 0; y < ConfigManager.S.Config.BaseSurfaceEmpty.y; y++)
            {
                List<TileState> _elems = new List<TileState>();
                for (int x = 0; x < ConfigManager.S.Config.BaseSurfaceEmpty.x; x++)
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

                for (int i = 0; i < ConfigManager.S.Config.StartingSubofficers; i++)
                {
                    var commandant = Instantiate(_commandantPrefab,
                        r.GameObject.transform.position + Vector3.up * .5f + Vector3.back * .5f,
                        Quaternion.identity);
                    EventManager.S.Subscribe(commandant.GetComponent<Commandant>());
                }
                for (int i = 0; i < ConfigManager.S.Config.StartingWarehousemans; i++)
                {
                    var warehouseman = Instantiate(_warehousePrefab,
                        r.GameObject.transform.position + Vector3.up * .5f + Vector3.back * .5f,
                        Quaternion.identity);
                    EventManager.S.Subscribe(warehouseman.GetComponent<Warehouseman>());
                }

            }, false);

            ARoom leftC1 = AddRoom(new Vector2Int(7, 0), new Vector2Int(1, 1), Corridor, RoomType.EMPTY, firstRoom, null, false);
            ARoom leftC2 = AddRoom(new Vector2Int(8, 0), new Vector2Int(1, 1), Corridor, RoomType.EMPTY, leftC1, null, false);
            ARoom leftR = AddRoom(new Vector2Int(9, 0), new Vector2Int(2, 1), ReceptionRoom, RoomType.EMPTY, leftC2, null, false);

            ARoom rightC1 = AddRoom(new Vector2Int(4, 0), new Vector2Int(1, 1), Corridor, RoomType.EMPTY, firstRoom, null, false);
            ARoom rightC2 = AddRoom(new Vector2Int(3, 0), new Vector2Int(1, 1), Corridor, RoomType.EMPTY, rightC1, null, false);
            ARoom rightR = AddRoom(new Vector2Int(1, 0), new Vector2Int(2, 1), ReceptionRoom, RoomType.EMPTY, rightC2, null, false);

            // var roominfo = Rooms.First(ri => ri.Size.x == 2 && ri.Size.y == 2);
            // ARoom FifthRoom = AddRoom(new Vector2Int(0, 2), new Vector2Int(2, 2), roominfo.GameObject, RoomType.EMPTY, fourthRoom, null, true);

        }

        public void NotifyBluePrintFininshed(ARoom room)
        {
            Debug.Log("NOTIFYBPF");
            // get the Masterblueprint where this room come from
            foreach (var masterbp in MapMasterBlueprints)
            {
                if (masterbp.Contains(room))
                // we found it
                {
                    masterbp.NotifyBlueprintFinished(room);
                    return;
                }
            }
        }

        public void NotifyMasterBluePrintFininshed(MasterBlueprint mbp)
        {
            Debug.Log("NOTIGYMASTERBPF");
            MapMasterBlueprints.Remove(mbp);
        }
        public bool CanIBuildHere(Vector2Int position, Vector2Int size)
        {
            Debug.Log("CANIBUILDHERE");
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
            Debug.Log("GETALLTURRETS");
            List<ARoom> result = new List<ARoom>();
            return result;
        }

        public List<ARoom> GetAllFactory()
        {
            Debug.Log("GETALLFACTORY");
            List<ARoom> result = new List<ARoom>();
            return result;
        }

        public List<ARoom> GetAllAccessibleBlueprint()
        {
            Debug.Log("GETALLACCESSIBLEBLUEPRINT");
            var result = MapRooms.Where(
                r => r.Requirement != null
            ).Where(
                // check that at least one neighbor is not a blueprint
                r => r.GetNeighborhood().ToList().Where(nr => (nr.Requirement == null) && (nr.IsBuilt)).Count() > 0
            ).ToList();

            return result;
        }

        public List<ARoom> GetAllStockRoom()
        {
            Debug.Log("GETALLSTOCKROOM");
            List<ARoom> result = MapRooms.Where(
                r => r is GenericRoom gRoom && gRoom.RoomType.Stock != null
            ).ToList();
            return result;
        }

        private bool IsThisLiftValid(Vector2Int position, (int corridors, int depth) lift, bool toTheRight)
        {
            Debug.Log("ISTHISLIFTVALID");
            // Check for cliff if going left
            if ((!toTheRight) && (position.x - lift.corridors - 1 < 0))
            {
                return false;
            }
            // Check if not explorated zone if going right
            if (toTheRight)
            {
                // If expansion out of the grid => oups !
                if (position.x + lift.corridors + 1 > _mapPathfinding[position.y].Count)
                {
                    return false;
                }
            }
            // Now we check for other constructions
            if (toTheRight)
            {
                /**
                *    #########
                *   #XXXXX    #
                *   #    X    #
                *   #    X    #
                *   #         #
                *    #########
                */
                // First check for the corridors + base line
                for (int col = position.x; col < position.x + lift.corridors + 1; ++col)
                {
                    if (_mapPathfinding[position.y][col] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
                // Check the up line if any
                for (int line = position.y; line <= position.y + lift.depth; ++line)
                {
                    if (_mapPathfinding[line][position.x + lift.corridors] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
            }
            else
            {
                /**
                *    #########
                *   #    XXXXX#
                *   #    X    #
                *   #    X    #
                *   #         #
                *    #########
                */
                // First check for the corridors + base line
                for (int col = position.x - lift.corridors - 1; col < position.x; ++col)
                {
                    if (_mapPathfinding[position.y][col] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
                // Check the up line if any
                for (int line = position.y; line <= position.y + lift.depth; ++line)
                {
                    if (_mapPathfinding[line][position.x - lift.corridors - 1] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
            }
            return true;


        }

        private bool IsThisConstructionValid(Vector2Int position, (int corridors, int width, int heigth) construction, bool toTheRight)
        {
            Debug.Log("ISTHISCONSTRUCTIONVALID");
            // First check for out of the water
            if (position.y - construction.heigth + 1 < 0)
            {
                return false;
            }
            // Check for cliff if going left
            if ((!toTheRight) && (position.x - construction.corridors - construction.width < 0))
            {
                return false;
            }
            // Check if not explorated zone if going right
            if (toTheRight)
            {
                // Check every line
                for (int line = position.y - construction.heigth + 1; line <= position.y; ++line)
                {
                    // If expansion out of the grid => oups !
                    if (position.x + construction.corridors + construction.width > _mapPathfinding[line].Count)
                    {
                        return false;
                    }
                }
            }
            // Now we check for other constructions
            if (toTheRight)
            {
                /**
                *    #########
                *   #   XXX   #
                *   #XXXXXX   #
                *    #########
                */
                // First check for the corridors + base line
                for (int col = position.x; col < position.x + construction.corridors + construction.width; ++col)
                {
                    if (_mapPathfinding[position.y][col] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
                // Check the up line if any
                if (construction.heigth > 1)
                {
                    for (int line = position.y - construction.heigth + 1; line <= position.y - 1; ++line)
                    {
                        for (int col = position.x + construction.corridors + 1; col < position.x + construction.corridors + construction.width; ++col)
                        {
                            if (_mapPathfinding[line][col] == TileState.OCCUPIED)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                /**
                *    #########
                *   #   XXX   #
                *   #   XXXXXX#
                *    #########
                */
                for (int col = position.x - construction.corridors - construction.width; col < position.x; ++col)
                {
                    if (_mapPathfinding[position.y][col] == TileState.OCCUPIED)
                    {
                        return false;
                    }
                }
                // Check the up line if any
                if (construction.heigth > 1)
                {
                    for (int line = position.y - construction.heigth + 1; line <= position.y - 1; ++line)
                    {
                        for (int col = position.x - construction.corridors - construction.width; col < position.x - construction.corridors - 1; ++col)
                        {
                            if (_mapPathfinding[line][col] == TileState.OCCUPIED)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public List<(int corridors, int width, int heigth)> GetZoneConstructionPossibilities(Vector2Int position, bool toTheRight, int corridorMin, int corridorMax)
        {
            Debug.Log("GETZONECONSTRUCTION");
            List<(int corridors, int width, int heigth)> result = new List<(int corridors, int width, int heigth)>();
            foreach (var room in _possibleRoomsSize)
            {
                for (int corridor = corridorMin; corridor <= corridorMax; ++corridor)
                {
                    if (IsThisConstructionValid(position, (corridor, room.width, room.heigth), toTheRight))
                    {
                        result.Add((corridor, room.width, room.heigth));
                    }
                }
            }

            return result;
        }

        public List<(int corridors, int depth)> GetZoneLiftPossibilities(Vector2Int position, bool toTheRight, int corridorMin, int corridorMax)
        {
            Debug.Log("GETZONELIFT");
            // check for depth from 1 to 3 !!
            List<(int corridors, int depth)> result = new List<(int corridors, int depth)>();
            for (int depth = 1; depth <= 3; ++depth)
            {
                for (int corridor = corridorMin; corridor <= corridorMax; ++corridor)
                {
                    if (IsThisLiftValid(position, (corridor, depth), toTheRight))
                    {
                        result.Add((corridor, depth));
                    }
                }
            }
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
            Debug.Log("ADDROOM");
            ARoom newRoom;
            switch (type)
            {
                case RoomType.ELEVATOR:
                    newRoom = new LiftRoom(size, position);
                    break;
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

            var go = Instantiate(room, new Vector2(position.x, -position.y) + new Vector2(1f, -1f), Quaternion.identity);
            go.transform.parent = _mapTransform;
            newRoom.GameObject = go;

            if (type == RoomType.CORRIDOR)
            {
                go.transform.rotation = Quaternion.Euler(22.3f, 0f, -90f);
                go.transform.position += new Vector3(-.911f, .361f, -.555f);
            }

            if (Application.isEditor) // Editor debug
            {
                var dText = Instantiate(_debugText, go.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                dText.GetComponent<TextMesh>().text = "(" + newRoom.Position.x + ", " + newRoom.Position.y + ")";
            }

            for (int y = position.y - size.y + 1; y <= position.y; ++y)
            {
                for (int x = position.x; x < position.x + size.x; ++x)
                {
                    _mapPathfinding[y][x] = TileState.OCCUPIED;
                }
            }

            // foreach (var room in MapRooms)
            // {
            //     if (room.Position.y == position.y && room.Position.x + room.Size.x == position.x)
            //     // our left neigbor
            //     {
            //         room.RoomRight = newRoom;
            //         newRoom.RoomLeft = room;
            //     }
            //     else if (room.Position.y == position.y && position.x + size.x == room.Position.x)
            //     {
            //         newRoom.RoomRight = room;
            //         room.RoomLeft = newRoom;
            //     }

            //     // if we are a lift or corridor and our top or bottom neigh is a corridor
            //     if ((type == RoomType.ELEVATOR || type == RoomType.CORRIDOR) && (type == RoomType.ELEVATOR || type == RoomType.CORRIDOR))
            //     {

            //     }

            // }

            MapRooms.Add(newRoom);

            // check where the parent room come from
            if (position.x > parentRoom.Position.x)
            // Comming from left
            {
                parentRoom.RoomRight = newRoom;
                newRoom.RoomLeft = parentRoom;
            }
            else if (position.x < parentRoom.Position.x)
            // Comming from right
            {
                parentRoom.RoomLeft = newRoom;
                newRoom.RoomRight = parentRoom;
            }

            // Special case for Y => Only 1x1 connection from top <-> bottom from now
            else if (position.y < parentRoom.Position.y)
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
                StartCoroutine(BuildRoom(newRoom, roomBuilt));
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
                newRoom.Requirement = new Requirement(newRoom, Enumerable.Repeat(ConfigManager.S.Config.RequirementPerBloc, newRoom.Size.x * newRoom.Size.y).SelectMany(x => x).ToArray(), false);
            }

            return newRoom;
        }


        public void BuildRoomExt(ARoom room)
        {
            Debug.Log("BUILDROOMEXT");
            StartCoroutine(BuildRoom(room, null));
        }
        private IEnumerator BuildRoom(ARoom room, Action<ARoom> roomBuilt)
        {
            Debug.Log("BUILDROOM");
            if (room.Sign != null)
            {
                Destroy(room.Sign);
                room.Sign = null;
            }
            room.Requirement = null;
            var sign = Instantiate(_constructionSign, (Vector3)(new Vector2(room.Position.x, -room.Position.y)) + new Vector3(room.Size.x / 2, -room.Size.y / 2f, -1f), Quaternion.identity);
            yield return new WaitForSeconds(ConfigManager.S.Config.BuildingTime);
            room.IsBuilt = true;
            Destroy(sign);
            roomBuilt?.Invoke(room);
            EventManager.S.NotifyManager(Events.Event.RoomCreated, room);
        }

        public void DiscoverTile(int x, int y)
        {
            Debug.Log("DISCOVERTILE");
            if (x < 0 || y < 0)
            {
                return;
            }
            if (y >= _mapPathfinding.Count || x >= _mapPathfinding[y].Count)
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
            }
            if (_mapPathfinding[y][x] == TileState.NOT_DISCOVERED)
            {
                _mapPathfinding[y][x] = TileState.EMPTY;
                EventManager.S.NotifyManager(Events.Event.ExplorationNewZone, new Vector2Int(x, y));
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
