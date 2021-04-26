using System.Collections.Generic;
using System.Linq;
using Scripts.Map;
using Scripts.Map.Room;
using UnityEngine;
using Scripts.Map.Blueprints;
using Scripts.Events;
using Scripts.ScriptableObjects;

namespace Scripts.Agents
{

    class Commandant : AAgent
    {
        public Commandant() : base("Sub-officer")
        { }

        protected override void DoStartAction()
        {
            GenerateNewMasterBlueprint();
        }

        private void GenerateNewMasterBlueprint()
        {
            // Choose between Expansion or Descent
            bool descent = Random.Range(0f, 1f) < ConfigManager.S.Config.ChangeBuildElevator;
            if (descent)
            {
                // choose the layer at the bottom
                int bottomLayer = MapManager.S.MapRooms.Select(room => room.Position.y).Max();
                // try to place a lift at the right or left of each room in this layer
                var rrls = MapManager.S.MapRooms.Where(
                    room => room.Position.y == bottomLayer && (room.RoomLeft == null || room.RoomRight == null)
                ).Select(
                    room => (room, MapManager.S.GetZoneLiftPossibilities(room.Position + new Vector2Int(room.Size.x, 0), true, 1, 6), MapManager.S.GetZoneLiftPossibilities(room.Position, false, 1, 6))
                ).Where(
                    rrl => (rrl.Item2.Count > 0 || rrl.Item3.Count > 0)
                ).ToList();

                if (rrls.Count > 0)
                {

                    var rrl = rrls[Random.Range(0, rrls.Count)];

                    List<int> directionChoices = new List<int>();
                    if (rrl.Item2.Count > 0)
                    {
                        directionChoices.Add(0);
                    }
                    if (rrl.Item3.Count > 0)
                    {
                        directionChoices.Add(1);
                    }

                    int direction = directionChoices[Random.Range(0, directionChoices.Count)];

                    var blueprints = (direction == 0) ? rrl.Item2 : rrl.Item3;

                    var blueprint = blueprints[Random.Range(0, blueprints.Count)];

                    ARoom runningRoom = rrl.Item1;
                    List<ARoom> roomList = new List<ARoom>();
                    Debug.Log("Direction : " + ((direction == 0) ? "Rigth" : "Left") + " ------------------------------------------------------");
                    if (direction == 0)
                    {
                        for (int corridor = 0; corridor < blueprint.Item1; ++corridor)
                        {
                            runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x + runningRoom.Size.x, runningRoom.Position.y),
                                new Vector2Int(1, 1),
                                MapManager.S.Corridor,
                                RoomType.CORRIDOR,
                                runningRoom,
                                null,
                                true
                            );
                            roomList.Add(
                                runningRoom
                            );
                        }
                        runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x + runningRoom.Size.x, runningRoom.Position.y),
                                new Vector2Int(1, 1),
                                MapManager.S.Elevator,
                                RoomType.ELEVATOR,
                                runningRoom,
                                null,
                                true
                            );
                        roomList.Add(
                            runningRoom
                        );
                        for (int depth = 0; depth < blueprint.Item2 - 1; ++depth)
                        {
                            runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x, runningRoom.Position.y + 1),
                                new Vector2Int(1, 1),
                                MapManager.S.Corridor,
                                RoomType.CORRIDOR,
                                runningRoom,
                                null,
                                true
                            );
                            roomList.Add(
                                runningRoom
                            );
                        }
                        runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x, runningRoom.Position.y + 1),
                                new Vector2Int(1, 1),
                                MapManager.S.Elevator,
                                RoomType.ELEVATOR,
                                runningRoom,
                                null,
                                true
                            );
                        roomList.Add(
                            runningRoom
                        );
                    }
                    else
                    {
                        for (int corridor = 0; corridor < blueprint.Item1; ++corridor)
                        {
                            runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x - 1, runningRoom.Position.y),
                                new Vector2Int(1, 1),
                                MapManager.S.Corridor,
                                RoomType.CORRIDOR,
                                runningRoom,
                                null,
                                true
                            );
                            roomList.Add(
                                runningRoom
                            );
                        }
                        runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x - runningRoom.Size.x, runningRoom.Position.y),
                                new Vector2Int(1, 1),
                                MapManager.S.Elevator,
                                RoomType.ELEVATOR,
                                runningRoom,
                                null,
                                true
                            );
                        roomList.Add(
                            runningRoom
                        );
                        for (int depth = 0; depth < blueprint.Item2 - 1; ++depth)
                        {
                            runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x, runningRoom.Position.y + 1),
                                new Vector2Int(1, 1),
                                MapManager.S.Corridor,
                                RoomType.CORRIDOR,
                                runningRoom,
                                null,
                                true
                            );
                            roomList.Add(
                                runningRoom
                            );
                        }
                        runningRoom = MapManager.S.AddRoom(
                                new Vector2Int(runningRoom.Position.x, runningRoom.Position.y + 1),
                                new Vector2Int(1, 1),
                                MapManager.S.Elevator,
                                RoomType.ELEVATOR,
                                runningRoom,
                                null,
                                true
                            );
                        roomList.Add(
                            runningRoom
                        );
                    }

                    // Debug.Break();

                    MapManager.S.MapMasterBlueprints.Add(new Map.Blueprints.MasterBlueprint(
                                roomList.Select(room => new Blueprint(room)).ToList(),
                                _id
                            ));

                    EventManager.S.NotifyManager(Events.Event.BlueprintDrawn, this);


                    return;
                }
                // if not possible, try building a classic room
            }
            {
                // for each room get the list of all possible expansions
                var rprl = MapManager.S.MapRooms.Where(
                    room => room.RoomLeft == null || room.RoomRight == null
                ).Select(
                    room => (room, (float)room.Position.x + 3 * room.Position.y, MapManager.S.GetZoneConstructionPossibilities(room.Position + new Vector2Int(room.Size.x, 0), true, 1, 6), MapManager.S.GetZoneConstructionPossibilities(room.Position, false, 1, 2))
                ).Where(
                    rprl => (rprl.Item3.Count > 0) || (rprl.Item4.Count > 0)
                ).ToList();

                if (rprl.Count == 0)
                {
                    return;
                }

                float total = 0;
                foreach (var rprlVal in rprl)
                {
                    total += rprlVal.Item2;
                }

                int i = 0;
                {
                    float choice = Random.Range(0f, total) - rprl[0].Item2;
                    while (choice > 0f)
                    {
                        ++i;
                        choice -= rprl[i].Item2;
                    }
                }

                // for each expansion, associate a probability weigth
                var choosenRoom = rprl[i];

                Debug.Log("ChoosenRoom lists : " + choosenRoom.Item3.Count + ", " + choosenRoom.Item4.Count);

                List<int> directionChoices = new List<int>();
                if (choosenRoom.Item3.Count > 0)
                {
                    directionChoices.Add(0);
                }
                if (choosenRoom.Item4.Count > 0)
                {
                    directionChoices.Add(1);
                }

                Debug.Log("DirectionChoices : " + directionChoices.Count);

                int direction = directionChoices[Random.Range(0, directionChoices.Count)];

                var blueprints = (direction == 0) ? choosenRoom.Item3 : choosenRoom.Item4;

                var blueprint = blueprints[Random.Range(0, blueprints.Count)];

                ARoom runningRoom = choosenRoom.Item1;

                List<ARoom> roomList = new List<ARoom>();
                Debug.Log("Direction : " + ((direction == 0) ? "Rigth" : "Left") + " ------------------------------------------------------");
                if (direction == 0)
                {
                    for (int corridor = 0; corridor < blueprint.Item1; ++corridor)
                    {
                        runningRoom = MapManager.S.AddRoom(
                            new Vector2Int(runningRoom.Position.x + runningRoom.Size.x, runningRoom.Position.y),
                            new Vector2Int(1, 1),
                            MapManager.S.Corridor,
                            RoomType.CORRIDOR,
                            runningRoom,
                            null,
                            true
                        );
                        roomList.Add(
                            runningRoom
                        );
                    }
                    var roominfo = MapManager.S.Rooms.First(ri => ri.Size.x == blueprint.Item2 && ri.Size.y == blueprint.Item3);
                    runningRoom = MapManager.S.AddRoom(
                            new Vector2Int(runningRoom.Position.x + runningRoom.Size.x, runningRoom.Position.y),
                            new Vector2Int(blueprint.Item2, blueprint.Item3),
                            roominfo.GameObject,
                            RoomType.EMPTY,
                            runningRoom,
                            null,
                            true
                        );
                    roomList.Add(
                        runningRoom
                    );
                }
                else
                {
                    for (int corridor = 0; corridor < blueprint.Item1; ++corridor)
                    {
                        runningRoom = MapManager.S.AddRoom(
                            new Vector2Int(runningRoom.Position.x - 1, runningRoom.Position.y),
                            new Vector2Int(1, 1),
                            MapManager.S.Corridor,
                            RoomType.CORRIDOR,
                            runningRoom,
                            null,
                            true
                        );
                        roomList.Add(
                            runningRoom
                        );
                    }
                    var roominfo = MapManager.S.Rooms.First(ri => ri.Size.x == blueprint.Item2 && ri.Size.y == blueprint.Item3);
                    runningRoom = MapManager.S.AddRoom(
                            new Vector2Int(runningRoom.Position.x - blueprint.Item2, runningRoom.Position.y),
                            new Vector2Int(blueprint.Item2, blueprint.Item3),
                            roominfo.GameObject,
                            RoomType.EMPTY,
                            runningRoom,
                            null,
                            true
                        );
                    roomList.Add(
                        runningRoom
                    );

                }

                // Debug.Break();

                MapManager.S.MapMasterBlueprints.Add(new Map.Blueprints.MasterBlueprint(
                            roomList.Select(room => new Blueprint(room)).ToList(),
                            _id
                        ));

                EventManager.S.NotifyManager(Events.Event.BlueprintDrawn, this);
            }
        }

        public override bool ChooseAction()
        {
            return false;
        }

        public override void DoSpecialAction(Action action)
        {
        }

        public override void OnEventReceived(Events.Event e, object o)
        {
            if (e == Events.Event.MasterBlueprintFinished)
            {
                MasterBlueprint mbp = (MasterBlueprint)o;

                if (mbp.Owner == _id)
                {
                    // this is our BP
                    GenerateNewMasterBlueprint();
                }
            }
            if (e == Events.Event.ExplorationNewZone)
            {
                // check if any blueprints is active
                if (!MapManager.S.MapMasterBlueprints.Any(mbp => mbp.Owner == _id))
                {
                    GenerateNewMasterBlueprint();
                }
            }

        }

    }

}
