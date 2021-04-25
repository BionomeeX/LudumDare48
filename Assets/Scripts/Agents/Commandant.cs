using System.Collections;
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

        public Commandant() : base("Commandant")
        {

        }

        protected override void DoStartAction()
        {
            GenerateNewMasterBlueprint();
        }

        private void GenerateNewMasterBlueprint()
        {
            // choose a room
            // 1) choose a room from where to expand
            List<(ARoom room, float weight)> choices = new List<(ARoom room, float weight)>();
            foreach (ARoom room in MapManager.S.MapRooms)
            {
                if (room.RoomLeft == null || room.RoomRight == null)
                {
                    choices.Add((room, (float)room.Position.x + 3 * room.Position.y));
                }
            }

            // if there is no room to expand, wait for the next time the map change
            if (choices.Count == 0)
            {
                return;
            }

            float total = 0;
            foreach ((var _, var weight) in choices)
            {
                total += weight;
            }

            int i = 0;
            {
                float choice = Random.Range(0f, total) - choices[0].weight;
                while (choice > 0f)
                {
                    ++i;
                    choice -= choices[i].weight;
                }
            }
            // i is the choosen room
            ARoom targetRoom = choices[i].room;

            // choose a direction
            List<int> directions = new List<int>();
            if (targetRoom.RoomRight == null)
            {
                directions.Add(0);
            }
            if (targetRoom.RoomLeft == null)
            {
                directions.Add(1);
            }

            // choose one
            {
                int choice = directions[Random.Range(0, directions.Count)];

                List<ARoom> roomList = new List<ARoom>();

                roomList.Add(
                    MapManager.S.AddRoom(
                        new Vector2Int(targetRoom.Position.x + ((choice == 0) ? +targetRoom.Size.x : -1), targetRoom.Position.y),
                        new Vector2Int(1, 1),
                        MapManager.S.Corridor,
                        RoomType.CORRIDOR,
                        targetRoom,
                        null,
                        true
                    )
                );
                roomList.Add(
                    MapManager.S.AddRoom(
                        new Vector2Int(roomList[0].Position.x + ((choice == 0) ? +roomList[0].Size.x : -1), roomList[0].Position.y),
                        new Vector2Int(1, 1),
                        MapManager.S.Corridor,
                        RoomType.CORRIDOR,
                        roomList[0],
                        null,
                        true
                    )
                );
                roomList.Add(
                    MapManager.S.AddRoom(
                        new Vector2Int(roomList[1].Position.x + ((choice == 0) ? +roomList[1].Size.x : -2), roomList[1].Position.y),
                        new Vector2Int(1, 1),
                        MapManager.S.ReceptionRoom,
                        RoomType.EMPTY,
                        roomList[1],
                        null,
                        true
                    )
                );

                MapManager.S.MapMasterBlueprints.Add(new Map.Blueprints.MasterBlueprint(
                    roomList.Select(room => new Blueprint(room)).ToList(),
                    _id
                ));

                EventManager.S.NotifyManager(Events.Event.BlueprintDrawn, this);
            }
        }

        public override void ChooseAction()
        {
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
