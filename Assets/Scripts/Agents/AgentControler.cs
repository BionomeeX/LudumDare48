using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Map;
using Scripts.Map.Room;


namespace Scripts.Agents
{


    public class AgentControler : MonoBehaviour
    {

        private List<ARoom> _roomPath = new List<ARoom>();
        private ARoom _currentRoom = null;
        private ARoom _targetRoom = null;

        public enum Action
        {
            Idle = 1, Building = 2, Mining = 4, Moving = 8,
        }

        private Action _currentAction;

        public void OnMapChange()
        {
            if (_targetRoom != null)
            {
                _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
            }
        }

        public void MoveToRoom(ARoom target)
        {
            _currentRoom = target;
            transform.position = new Vector3(
                _currentRoom.Position.x + 0.5f,
                -_currentRoom.Position.y + 0.1f,
                0f
            );
        }

        private void Start()
        {
            StartCoroutine(ChooseAction());
        }



        private IEnumerator ChooseAction( /*??*/ )
        {
            while (true)
            {
                Debug.Log("ChooseAction begin");
                // First of all, if _currentRoom is null, let's try to find where we are
                if (_currentRoom == null)
                {
                    Debug.Log("  Checking for room");
                    Debug.Log("  My pos : " + transform.position.x + ", " + transform.position.y);
                    foreach (var room in MapManager.S.MapRooms)
                    {
                        Debug.Log("    Room pos :");
                        Debug.Log("      (" + room.Position.x + ", " + room.Position.y + ") (" + (room.Position.x + room.Size.x) + ", " + (room.Position.y + room.Size.y) + ")");

                        if (transform.position.x >= room.Position.x && transform.position.x <= room.Position.x + room.Size.x &&
                           -transform.position.y >= room.Position.y && -transform.position.y <= room.Position.y + room.Size.y
                        )
                        {
                            _currentRoom = room;
                            break;
                        }
                    }
                    // if _currentRoom is still null ... we are in the water !
                    if (_currentRoom == null)
                    {
                        Debug.Log("  Still no room => aborting");
                        yield return new WaitForSeconds(1f);
                    }
                }

                if (_currentAction == Action.Idle)
                {
                    Debug.Log("  Currently Idle");

                    float actionChoice = Random.Range(0f, 1f);

                    if (actionChoice < 0.9f)
                    // 10% chance build a new room
                    {
                        Debug.Log("    Will Build");
                        // go to building mode
                        // 1) choose a room from where to expand
                        List<(ARoom room, float weight)> choices = new List<(ARoom room, float weight)>();
                        foreach (ARoom room in MapManager.S.MapRooms)
                        {
                            if (room.RoomLeft == null || room.RoomRight == null)
                            {
                                choices.Add((room, (float)room.Position.x + 3 * room.Position.y));
                            }
                        }

                        // if there is room to expand
                        if (choices.Count > 0)
                        {
                            float total = 0;
                            foreach ((var _, var weight) in choices)
                            {
                                total += weight;
                            }

                            float choice = Random.Range(0f, total) - choices[0].weight;
                            int i = 0;
                            while (choice > 0f)
                            {
                                ++i;
                                choice -= choices[i].weight;
                            }

                            // i is the choosen room
                            _targetRoom = choices[i].room;
                            // 2) set the path to the target room
                            _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
                            Debug.Log("    Going to room (" + _targetRoom.Position.x + ", " + _targetRoom.Position.y + ") to build");
                            _currentAction = Action.Building;
                        }
                    }
                    else
                    {
                        // move to a random room
                        _targetRoom = MapManager.S.MapRooms[Random.Range(0, MapManager.S.MapRooms.Count)];
                        _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
                        Debug.Log("    Going to room (" + _targetRoom.Position.x + ", " + _targetRoom.Position.y + ")");
                    }

                }
                else if (_currentAction == Action.Building)
                {
                    Debug.Log("  Currently Building");
                    if (_roomPath.Count > 0)
                    {
                        // still moving
                        var room = _roomPath[0];
                        Debug.Log("    Still Moving to (" + room.Position.x + ", " + room.Position.y + ")");
                        // Go to next room
                        MoveToRoom(room);
                        _roomPath.RemoveAt(0);
                    }
                    else
                    {
                        Debug.Log("    Building Room/Corridor");
                        // We are at the building room
                        _targetRoom = null;
                        // now start building
                        if (_currentRoom is Corridor)
                        {
                            Debug.Log("      In a corridor");
                            if (Random.Range(0f, 1f) < 0.75f)
                            // 75% expand corridor
                            {
                                Debug.Log("        Expanding corridor");
                                Debug.Log("          In the same direction");
                                ARoom corridor = null;
                                if (_currentRoom.RoomLeft != null)
                                // we are going from left to right
                                {
                                    corridor = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x + 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom, null, false);
                                }
                                else
                                // right to left
                                {
                                    corridor = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x - 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom, null, false);
                                }
                                _targetRoom = corridor;
                                _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
                            }
                            else
                            // 25% build a room
                            {
                                Debug.Log("        Build a new room");
                                ARoom room = null;
                                if (_currentRoom.RoomLeft != null)
                                // we are going from left to right
                                {
                                    Debug.Log("          To the right");
                                    room = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x + 1, _currentRoom.Position.y), new Vector2Int(2, 1), MapManager.S.ReceptionRoom, RoomType.EMPTY, _currentRoom, null, false);
                                }
                                else
                                // right to left
                                {
                                    Debug.Log("          To the left");
                                    room = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x - 2, _currentRoom.Position.y), new Vector2Int(2, 1), MapManager.S.ReceptionRoom, RoomType.EMPTY, _currentRoom, null, false);
                                }
                                _currentAction = Action.Idle;
                                _targetRoom = room;
                                _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
                            }
                        }
                        else
                        // not a corridor => we are in a room and we start a corridor
                        {
                            Debug.Log("      In a room");
                            // Right/Left??
                            List<int> directions = new List<int>();
                            if (_currentRoom.RoomRight == null)
                            {
                                directions.Add(0);
                            }
                            if (_currentRoom.RoomLeft == null)
                            {
                                directions.Add(1);
                            }

                            if (directions.Count > 0)
                            {
                                Debug.Log("        Building Corridor");
                                // choose one
                                int choice = directions[Random.Range(0, directions.Count)];
                                if (choice == 0)
                                // add to the right
                                {
                                    Debug.Log("          to the right");
                                    var corridor = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x + _currentRoom.Size.x, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom, null, false);
                                    _targetRoom = corridor;
                                    _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
                                }
                                else
                                // add to the left
                                {
                                    Debug.Log("          to the left");
                                    var corridor = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x - 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom, null, false);
                                    _targetRoom = corridor;
                                    _roomPath = Astar.FindPath(_currentRoom, _targetRoom);
                                }
                            }
                        }
                    }
                }
                else if (_roomPath.Count > 0)
                {
                    Debug.Log("  Currently Moving");
                    var room = _roomPath[0];
                    Debug.Log("    to (" + room.Position.x + ", " + room.Position.y + ")");
                    // we are moving
                    MoveToRoom(room);
                    _roomPath.RemoveAt(0);
                }
                else
                {
                    Debug.Log("  Currently Stopped Moving");
                    // we stopped moving ...
                    _targetRoom = null;
                    _currentAction = Action.Idle;
                }
                Debug.Log("ChooseAction End, starting anew ...");
                yield return new WaitForSeconds(0.1f);
                // ChooseAction();
            }
        }

    }

}
