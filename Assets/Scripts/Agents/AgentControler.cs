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
        private ARoom _currentRoom;
        private ARoom _targetRoom;

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
                _currentRoom.Position.y + 0.1f,
                0f
            );
        }

        private void Start()
        {
            ChooseAction();
        }

        private void ChooseAction( /*??*/ )
        {
            if (_currentAction == Action.Idle)
            {
                float actionChoice = Random.Range(0f, 1f);

                if (actionChoice < 0.1f)
                // 10% chance build a new room
                {
                    // go to building mode
                    // 1) choose a room from where to expand
                    List<(ARoom room, float weight)> choices = new List<(ARoom room, float weight)>();
                    foreach (ARoom room in MapManager.S.MapRooms)
                    {
                        if (room.GetNeighborhood().Length > 0)
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

                        _currentAction = Action.Building;
                    }
                }
                else
                {
                    // move to a random room
                    _targetRoom = MapManager.S.MapRooms[Random.Range(0, MapManager.S.MapRooms.Count)];
                    _roomPath = Astar.FindPath(_currentRoom, _targetRoom);

                }

            }
            else if (_currentAction == Action.Building)
            {
                if (_roomPath.Count > 0)
                {
                    // still moving
                    // Go to next room
                    MoveToRoom(_roomPath[0]);
                    _roomPath.RemoveAt(0);
                }
                else
                {
                    // We are at the building room
                    _targetRoom = null;
                    // now start building
                    if (_currentRoom is Corridor)
                    {
                        if (Random.Range(0f, 1f) < 0.75f)
                        // 75% expand corridor
                        {
                            float rnd = Random.Range(0f, 1f);
                            if (rnd < 0.5f)
                            // 50% same direction
                            {
                                if (_currentRoom.RoomRight == null)
                                // we are going from left to right
                                {
                                    var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x + 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom);
                                }
                                else
                                // right to left
                                {
                                    var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x - 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom);
                                }
                            }
                            else if (rnd < 0.75f)
                            // 25% top
                            {
                                var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x, _currentRoom.Position.y + 1), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom);
                            }
                            else
                            // 25% bottom
                            {
                                var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x, _currentRoom.Position.y - 1), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom);
                            }
                        }
                        else
                        // 25% build a room
                        {
                            if (_currentRoom.RoomRight == null)
                            // we are going from left to right
                            {
                                var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x + 1, _currentRoom.Position.y), new Vector2Int(2, 1), MapManager.S.ReceptionRoom, RoomType.EMPTY, _currentRoom);
                            }
                            else
                            // right to left
                            {
                                var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x - 2, _currentRoom.Position.y), new Vector2Int(2, 1), MapManager.S.ReceptionRoom, RoomType.EMPTY, _currentRoom);
                            }

                        }
                    }
                    else
                    // not a corridor => we are in a room and we start a corridor
                    {
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

                        // choose one
                        int choice = Random.Range(0, directions.Count);
                        if (choice == 0)
                        // add to the right
                        {
                            var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x + _currentRoom.Size.x + 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom);
                        }
                        else
                        // add to the left
                        {
                            var _ = MapManager.S.AddRoom(new Vector2Int(_currentRoom.Position.x - 1, _currentRoom.Position.y), new Vector2Int(1, 1), MapManager.S.Corridor, RoomType.CORRIDOR, _currentRoom);
                        }

                        _currentAction = Action.Idle;
                    }

                }
            }
            else if (_roomPath.Count > 0)
            {
                // we are moving
                MoveToRoom(_roomPath[0]);
                _roomPath.RemoveAt(0);
            }
            else
            {
                // we stopped moving ...
                _targetRoom = null;
            }
            //

            return;
        }

    }

}
