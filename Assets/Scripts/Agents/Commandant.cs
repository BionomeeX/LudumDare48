using System.Collections.Generic;
using System.Linq;
using Scripts.Map;
using Scripts.Map.Room;
using UnityEngine;
using Scripts.Map.Blueprints;
using Scripts.Events;


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
            // for each room get the list of all possible expansions
            var rprl = MapManager.S.MapRooms.Where(
                room => room.RoomLeft == null || room.RoomRight == null
            ).Select(
                room => (room, (float)room.Position.x + 3 * room.Position.y, MapManager.S.GetZoneConstructionPossibilities(room.Position, true, 1, 6), MapManager.S.GetZoneConstructionPossibilities(room.Position, false, 1, 6))
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
                        new Vector2Int(runningRoom.Position.x + 1, runningRoom.Position.y),
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
                        new Vector2Int(runningRoom.Position.x + 1, runningRoom.Position.y),
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
            } else {
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

            Debug.Break();

            MapManager.S.MapMasterBlueprints.Add(new Map.Blueprints.MasterBlueprint(
                        roomList.Select(room => new Blueprint(room)).ToList(),
                        _id
                    ));

            EventManager.S.NotifyManager(Events.Event.BlueprintDrawn, this);


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
        }

    }

}
